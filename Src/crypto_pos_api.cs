using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CryptoComPaymentService>();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// API Endpoints
app.MapPost("/api/payment/create", async (
    [FromBody] CreatePaymentRequest request,
    [FromServices] CryptoComPaymentService service) =>
{
    try
    {
        var result = await service.CreatePaymentAsync(request);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreatePayment")
.WithOpenApi();

app.MapGet("/api/payment/{orderId}", async (
    string orderId,
    [FromServices] CryptoComPaymentService service) =>
{
    try
    {
        var result = await service.GetPaymentStatusAsync(orderId);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GetPaymentStatus")
.WithOpenApi();

app.MapPost("/api/webhook/payment", async (
    HttpContext context,
    [FromServices] CryptoComPaymentService service) =>
{
    try
    {
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        
        var signature = context.Request.Headers["X-Pay-Signature"].ToString();
        
        if (!service.VerifyWebhookSignature(body, signature))
        {
            return Results.Unauthorized();
        }

        var webhook = JsonSerializer.Deserialize<WebhookPayload>(body);
        
        // Process webhook (implement your business logic)
        Console.WriteLine($"Payment webhook received for order: {webhook?.OrderId}");
        Console.WriteLine($"Status: {webhook?.Status}");
        
        return Results.Ok(new { received = true });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("PaymentWebhook")
.WithOpenApi();

app.MapGet("/api/currencies", async ([FromServices] CryptoComPaymentService service) =>
{
    try
    {
        var result = await service.GetSupportedCurrenciesAsync();
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GetSupportedCurrencies")
.WithOpenApi();

app.Run();

// Models
public record CreatePaymentRequest(
    string Currency,
    decimal Amount,
    string Description,
    string? OrderId = null,
    string? ReturnUrl = null,
    string? CancelUrl = null
);

public record PaymentResponse(
    string OrderId,
    string PaymentUrl,
    decimal Amount,
    string Currency,
    string Status,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);

public record PaymentStatusResponse(
    string OrderId,
    string Status,
    decimal Amount,
    string Currency,
    DateTime CreatedAt,
    DateTime? PaidAt
);

public record WebhookPayload(
    string OrderId,
    string Status,
    decimal Amount,
    string Currency,
    string? TransactionHash,
    DateTime Timestamp
);

// Service
public class CryptoComPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _secretKey;
    private readonly string _baseUrl;

    public CryptoComPaymentService(IConfiguration config)
    {
        _httpClient = new HttpClient();
        _apiKey = config["CryptoCom:ApiKey"] ?? "your_api_key_here";
        _secretKey = config["CryptoCom:SecretKey"] ?? "your_secret_key_here";
        _baseUrl = config["CryptoCom:BaseUrl"] ?? "https://pay.crypto.com/api";
    }

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var orderId = request.OrderId ?? Guid.NewGuid().ToString();
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var payload = new
        {
            order_id = orderId,
            currency = request.Currency,
            amount = request.Amount.ToString("F2"),
            description = request.Description,
            return_url = request.ReturnUrl,
            cancel_url = request.CancelUrl,
            timestamp
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var signature = GenerateSignature(jsonPayload);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/payments")
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };

        httpRequest.Headers.Add("X-Pay-Api-Key", _apiKey);
        httpRequest.Headers.Add("X-Pay-Signature", signature);

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API Error: {responseContent}");
        }

        // Parse response (adjust based on actual API response structure)
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

        return new PaymentResponse(
            OrderId: orderId,
            PaymentUrl: result.GetProperty("payment_url").GetString() ?? "",
            Amount: request.Amount,
            Currency: request.Currency,
            Status: "pending",
            CreatedAt: DateTime.UtcNow,
            ExpiresAt: DateTime.UtcNow.AddHours(1)
        );
    }

    public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string orderId)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var path = $"/payments/{orderId}";
        var signaturePayload = $"{path}{timestamp}";
        var signature = GenerateSignature(signaturePayload);

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}{path}");
        httpRequest.Headers.Add("X-Pay-Api-Key", _apiKey);
        httpRequest.Headers.Add("X-Pay-Signature", signature);
        httpRequest.Headers.Add("X-Pay-Timestamp", timestamp.ToString());

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API Error: {responseContent}");
        }

        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

        return new PaymentStatusResponse(
            OrderId: orderId,
            Status: result.GetProperty("status").GetString() ?? "unknown",
            Amount: decimal.Parse(result.GetProperty("amount").GetString() ?? "0"),
            Currency: result.GetProperty("currency").GetString() ?? "",
            CreatedAt: DateTime.Parse(result.GetProperty("created_at").GetString() ?? DateTime.UtcNow.ToString()),
            PaidAt: result.TryGetProperty("paid_at", out var paidAt) ? DateTime.Parse(paidAt.GetString() ?? "") : null
        );
    }

    public async Task<List<string>> GetSupportedCurrenciesAsync()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var path = "/currencies";
        var signaturePayload = $"{path}{timestamp}";
        var signature = GenerateSignature(signaturePayload);

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}{path}");
        httpRequest.Headers.Add("X-Pay-Api-Key", _apiKey);
        httpRequest.Headers.Add("X-Pay-Signature", signature);
        httpRequest.Headers.Add("X-Pay-Timestamp", timestamp.ToString());

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new List<string> { "USD", "EUR", "BTC", "ETH", "CRO", "USDT", "USDC" };
        }

        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        return result.GetProperty("currencies").EnumerateArray()
            .Select(c => c.GetString() ?? "")
            .ToList();
    }

    public bool VerifyWebhookSignature(string payload, string signature)
    {
        var expectedSignature = GenerateSignature(payload);
        return signature == expectedSignature;
    }

    private string GenerateSignature(string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLower();
    }
}
