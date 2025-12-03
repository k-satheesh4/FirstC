// ============================================================================
// PROJECT STRUCTURE:
// CryptoComPayIntegration/
// ├── Models/
// │   ├── PaymentModels.cs
// │   ├── RefundModels.cs
// │   └── WebhookModels.cs
// ├── Services/
// │   ├── ICryptoComPayService.cs
// │   ├── CryptoComPayService.cs
// │   └── WebhookService.cs
// ├── Controllers/
// │   └── PaymentController.cs (ASP.NET Core)
// ├── appsettings.json
// └── Program.cs
// ============================================================================

// ============================================================================
// FILE: Models/PaymentModels.cs
// ============================================================================
using System.Text.Json.Serialization;

namespace CryptoComPayIntegration.Models
{
    public class CreatePaymentRequest
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; }

        [JsonPropertyName("cancel_url")]
        public string CancelUrl { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [JsonPropertyName("onchain_allowed")]
        public bool? OnchainAllowed { get; set; }
    }

    public class PaymentResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("amount_refunded")]
        public int AmountRefunded { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("crypto_currency")]
        public string CryptoCurrency { get; set; }

        [JsonPropertyName("crypto_amount")]
        public string CryptoAmount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }

        [JsonPropertyName("payment_url")]
        public string PaymentUrl { get; set; }

        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; }

        [JsonPropertyName("cancel_url")]
        public string CancelUrl { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("live_mode")]
        public bool LiveMode { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("recipient")]
        public string Recipient { get; set; }

        [JsonPropertyName("refunded")]
        public bool Refunded { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("expired_at")]
        public long ExpiredAt { get; set; }

        public DateTime CreatedDateTime => DateTimeOffset.FromUnixTimeSeconds(Created).DateTime;
        public DateTime ExpiredDateTime => DateTimeOffset.FromUnixTimeSeconds(ExpiredAt).DateTime;
    }

    public class PaymentListResponse
    {
        [JsonPropertyName("items")]
        public List<PaymentResponse> Items { get; set; }

        [JsonPropertyName("meta")]
        public PaginationMeta Meta { get; set; }
    }

    public class PaginationMeta
    {
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("current_size")]
        public int CurrentSize { get; set; }

        [JsonPropertyName("next_page")]
        public int? NextPage { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
    }
}

// ============================================================================
// FILE: Models/RefundModels.cs
// ============================================================================
namespace CryptoComPayIntegration.Models
{
    public class CreateRefundRequest
    {
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class RefundResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("debit_currency")]
        public string DebitCurrency { get; set; }

        [JsonPropertyName("debit_amount")]
        public int DebitAmount { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("transferred_at")]
        public long? TransferredAt { get; set; }
    }
}

// ============================================================================
// FILE: Models/WebhookModels.cs
// ============================================================================
namespace CryptoComPayIntegration.Models
{
    public class WebhookEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object_type")]
        public string ObjectType { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("data")]
        public WebhookData Data { get; set; }
    }

    public class WebhookData
    {
        [JsonPropertyName("object")]
        public JsonElement Object { get; set; }
    }
}

// ============================================================================
// FILE: Services/ICryptoComPayService.cs
// ============================================================================
namespace CryptoComPayIntegration.Services
{
    public interface ICryptoComPayService
    {
        Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
        Task<PaymentResponse> GetPaymentAsync(string paymentId);
        Task<PaymentResponse> CancelPaymentAsync(string paymentId);
        Task<PaymentListResponse> ListPaymentsAsync(int page = 1, int perPage = 10);
        Task<RefundResponse> CreateRefundAsync(CreateRefundRequest request);
        Task<RefundResponse> GetRefundAsync(string refundId);
        Task<List<RefundResponse>> GetPaymentRefundsAsync(string paymentId);
    }
}

// ============================================================================
// FILE: Services/CryptoComPayService.cs
// ============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CryptoComPayIntegration.Services
{
    public class CryptoComPayService : ICryptoComPayService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CryptoComPayService> _logger;
        private const string BaseUrl = "https://pay.crypto.com/api";

        public CryptoComPayService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CryptoComPayService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var secretKey = configuration["CryptoCom:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("CryptoCom:SecretKey not configured");

            _httpClient.BaseAddress = new Uri(BaseUrl);
            
            // Setup Basic Authentication
            var authValue = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{secretKey}:"));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Basic", authValue);
        }

        public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            try
            {
                _logger.LogInformation("Creating payment for amount {Amount} {Currency}", 
                    request.Amount, request.Currency);

                var formData = new Dictionary<string, string>
                {
                    ["amount"] = request.Amount.ToString(),
                    ["currency"] = request.Currency,
                    ["description"] = request.Description ?? ""
                };

                if (!string.IsNullOrEmpty(request.OrderId))
                    formData["order_id"] = request.OrderId;
                if (!string.IsNullOrEmpty(request.ReturnUrl))
                    formData["return_url"] = request.ReturnUrl;
                if (!string.IsNullOrEmpty(request.CancelUrl))
                    formData["cancel_url"] = request.CancelUrl;
                if (request.OnchainAllowed.HasValue)
                    formData["onchain_allowed"] = request.OnchainAllowed.Value.ToString().ToLower();

                var content = new FormUrlEncodedContent(formData);
                var response = await _httpClient.PostAsync("/payments", content);
                
                await EnsureSuccessAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var payment = JsonSerializer.Deserialize<PaymentResponse>(json);

                _logger.LogInformation("Payment created successfully with ID: {PaymentId}", 
                    payment.Id);

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                throw;
            }
        }

        public async Task<PaymentResponse> GetPaymentAsync(string paymentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/payments/{paymentId}");
                await EnsureSuccessAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PaymentResponse>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment {PaymentId}", paymentId);
                throw;
            }
        }

        public async Task<PaymentResponse> CancelPaymentAsync(string paymentId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/payments/{paymentId}/cancel", null);
                await EnsureSuccessAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PaymentResponse>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment {PaymentId}", paymentId);
                throw;
            }
        }

        public async Task<PaymentListResponse> ListPaymentsAsync(int page = 1, int perPage = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"/payments?page={page}&per_page={perPage}");
                await EnsureSuccessAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PaymentListResponse>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing payments");
                throw;
            }
        }

        public async Task<RefundResponse> CreateRefundAsync(CreateRefundRequest request)
        {
            try
            {
                _logger.LogInformation("Creating refund for payment {PaymentId}", 
                    request.PaymentId);

                var formData = new Dictionary<string, string>
                {
                    ["payment_id"] = request.PaymentId,
                    ["amount"] = request.Amount.ToString()
                };

                if (!string.IsNullOrEmpty(request.Reason))
                    formData["reason"] = request.Reason;
                if (!string.IsNullOrEmpty(request.Description))
                    formData["description"] = request.Description;

                var content = new FormUrlEncodedContent(formData);
                var response = await _httpClient.PostAsync("/refunds", content);
                
                await EnsureSuccessAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var refund = JsonSerializer.Deserialize<RefundResponse>(json);

                _logger.LogInformation("Refund created successfully with ID: {RefundId}", 
                    refund.Id);

                return refund;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating refund");
                throw;
            }
        }

        public async Task<RefundResponse> GetRefundAsync(string refundId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/refunds/{refundId}");
                await EnsureSuccessAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RefundResponse>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refund {RefundId}", refundId);
                throw;
            }
        }

        public async Task<List<RefundResponse>> GetPaymentRefundsAsync(string paymentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/payments/{paymentId}/refunds");
                await EnsureSuccessAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<RefundResponse>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refunds for payment {PaymentId}", paymentId);
                throw;
            }
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API Error: {StatusCode} - {Content}", 
                    response.StatusCode, errorContent);
                throw new HttpRequestException(
                    $"API request failed with status {response.StatusCode}: {errorContent}");
            }
        }
    }
}

// ============================================================================
// FILE: Services/WebhookService.cs
// ============================================================================
using System.Security.Cryptography;
using System.Text;

namespace CryptoComPayIntegration.Services
{
    public class WebhookService
    {
        private readonly ILogger<WebhookService> _logger;
        private readonly string _signatureSecret;

        public WebhookService(IConfiguration configuration, ILogger<WebhookService> logger)
        {
            _logger = logger;
            _signatureSecret = configuration["CryptoCom:SignatureSecret"];
        }

        public bool VerifyWebhookSignature(string paySignatureHeader, string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(paySignatureHeader))
                {
                    _logger.LogWarning("Missing Pay-Signature header");
                    return false;
                }

                // Parse the signature header
                var parts = paySignatureHeader.Split(',');
                string timestamp = null;
                string signature = null;

                foreach (var part in parts)
                {
                    var kvp = part.Split('=');
                    if (kvp.Length != 2) continue;

                    if (kvp[0] == "t")
                        timestamp = kvp[1];
                    else if (kvp[0] == "v1")
                        signature = kvp[1];
                }

                if (string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(signature))
                {
                    _logger.LogWarning("Invalid signature header format");
                    return false;
                }

                // Verify timestamp is recent (within 5 minutes)
                if (long.TryParse(timestamp, out var ts))
                {
                    var requestTime = DateTimeOffset.FromUnixTimeSeconds(ts);
                    var age = DateTimeOffset.UtcNow - requestTime;
                    if (Math.Abs(age.TotalMinutes) > 5)
                    {
                        _logger.LogWarning("Webhook timestamp too old or in future");
                        return false;
                    }
                }

                // Compute expected signature
                var signedPayload = $"{timestamp}.{requestBody}";
                var expectedSignature = ComputeHmacSha256(_signatureSecret, signedPayload);

                // Constant-time comparison
                return SecureCompare(signature, expectedSignature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying webhook signature");
                return false;
            }
        }

        private string ComputeHmacSha256(string secret, string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private bool SecureCompare(string a, string b)
        {
            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        public async Task<WebhookEvent> ParseWebhookEventAsync(string requestBody)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                return JsonSerializer.Deserialize<WebhookEvent>(requestBody, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing webhook event");
                throw;
            }
        }
    }
}

// ============================================================================
// FILE: Controllers/PaymentController.cs (ASP.NET Core MVC)
// ============================================================================
using Microsoft.AspNetCore.Mvc;

namespace CryptoComPayIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ICryptoComPayService _paymentService;
        private readonly WebhookService _webhookService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            ICryptoComPayService paymentService,
            WebhookService webhookService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _webhookService = webhookService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var payment = await _paymentService.CreatePaymentAsync(request);
                return Ok(new
                {
                    success = true,
                    payment_id = payment.Id,
                    payment_url = payment.PaymentUrl,
                    status = payment.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPayment(string paymentId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentAsync(paymentId);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("{paymentId}/cancel")]
        public async Task<IActionResult> CancelPayment(string paymentId)
        {
            try
            {
                var payment = await _paymentService.CancelPaymentAsync(paymentId);
                return Ok(new { success = true, status = payment.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("refund")]
        public async Task<IActionResult> CreateRefund([FromBody] CreateRefundRequest request)
        {
            try
            {
                var refund = await _paymentService.CreateRefundAsync(request);
                return Ok(new
                {
                    success = true,
                    refund_id = refund.Id,
                    status = refund.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating refund");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                // Read raw body
                using var reader = new StreamReader(Request.Body);
                var requestBody = await reader.ReadToEndAsync();

                // Get signature header
                var signatureHeader = Request.Headers["Pay-Signature"].ToString();

                // Verify signature
                if (!_webhookService.VerifyWebhookSignature(signatureHeader, requestBody))
                {
                    _logger.LogWarning("Invalid webhook signature");
                    return Unauthorized();
                }

                // Parse event
                var webhookEvent = await _webhookService.ParseWebhookEventAsync(requestBody);

                _logger.LogInformation("Received webhook event: {EventType}", webhookEvent.Type);

                // Handle different event types
                switch (webhookEvent.Type)
                {
                    case "payment.created":
                        await HandlePaymentCreated(webhookEvent);
                        break;
                    case "payment.captured":
                        await HandlePaymentCaptured(webhookEvent);
                        break;
                    case "payment.refund_transferred":
                        await HandleRefundTransferred(webhookEvent);
                        break;
                    default:
                        _logger.LogInformation("Unhandled event type: {EventType}", 
                            webhookEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling webhook");
                return StatusCode(500);
            }
        }

        private async Task HandlePaymentCreated(WebhookEvent webhookEvent)
        {
            _logger.LogInformation("Handling payment.created event");
            // Add your business logic here
            await Task.CompletedTask;
        }

        private async Task HandlePaymentCaptured(WebhookEvent webhookEvent)
        {
            _logger.LogInformation("Handling payment.captured event");
            // Add your business logic here (e.g., fulfill order)
            await Task.CompletedTask;
        }

        private async Task HandleRefundTransferred(WebhookEvent webhookEvent)
        {
            _logger.LogInformation("Handling refund transferred event");
            // Add your business logic here
            await Task.CompletedTask;
        }
    }
}

// ============================================================================
// FILE: Program.cs (ASP.NET Core 6+)
// ============================================================================
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register CryptoCom services
builder.Services.AddHttpClient<ICryptoComPayService, CryptoComPayService>();
builder.Services.AddScoped<WebhookService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// ============================================================================
// FILE: appsettings.json
// ============================================================================
/*
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "CryptoCom": {
    "SecretKey": "sk_test_YOUR_SECRET_KEY_HERE",
    "PublishableKey": "pk_test_YOUR_PUBLISHABLE_KEY_HERE",
    "SignatureSecret": "whsec_YOUR_WEBHOOK_SECRET_HERE",
    "IsTestMode": true
  }
}
*/

// ============================================================================
// USAGE EXAMPLE: Console Application
// ============================================================================
/*
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup DI container
        var services = new ServiceCollection();
        
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole());
        services.AddHttpClient<ICryptoComPayService, CryptoComPayService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var paymentService = serviceProvider.GetRequiredService<ICryptoComPayService>();
        
        // Create a payment
        var payment = await paymentService.CreatePaymentAsync(new CreatePaymentRequest
        {
            Amount = 5000, // $50.00
            Currency = "USD",
            Description = "Test Product",
            OrderId = "TEST-ORDER-001",
            ReturnUrl = "https://yoursite.com/success",
            CancelUrl = "https://yoursite.com/cancel"
        });
        
        Console.WriteLine($"Payment ID: {payment.Id}");
        Console.WriteLine($"Payment URL: {payment.PaymentUrl}");
        Console.WriteLine($"Status: {payment.Status}");
    }
}
*/