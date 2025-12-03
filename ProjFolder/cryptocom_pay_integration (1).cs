// ==================== CryptoComPay.csproj ====================
/*
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
</Project>
*/

// ==================== Models/Payment.cs ====================
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CryptoComPay.Models
{
    public class Payment
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("amount")]
        public long Amount { get; set; }
        
        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;
        
        [JsonProperty("description")]
        public string? Description { get; set; }
        
        [JsonProperty("status")]
        public string? Status { get; set; }
        
        [JsonProperty("payment_url")]
        public string? PaymentUrl { get; set; }
        
        [JsonProperty("return_url")]
        public string? ReturnUrl { get; set; }
        
        [JsonProperty("cancel_url")]
        public string? CancelUrl { get; set; }
        
        [JsonProperty("order_id")]
        public string? OrderId { get; set; }
        
        [JsonProperty("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        
        [JsonProperty("crypto_currency")]
        public string? CryptoCurrency { get; set; }
        
        [JsonProperty("crypto_amount")]
        public string? CryptoAmount { get; set; }
        
        [JsonProperty("created")]
        public long? Created { get; set; }
        
        [JsonProperty("expired_at")]
        public long? ExpiredAt { get; set; }
        
        [JsonProperty("amount_refunded")]
        public long? AmountRefunded { get; set; }
        
        [JsonProperty("refunded")]
        public bool? Refunded { get; set; }
    }
    
    public class CreatePaymentRequest
    {
        public long Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Description { get; set; }
        public string? OrderId { get; set; }
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }
}

// ==================== Models/Refund.cs ====================
namespace CryptoComPay.Models
{
    public class Refund
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("payment_id")]
        public string PaymentId { get; set; } = string.Empty;
        
        [JsonProperty("amount")]
        public long Amount { get; set; }
        
        [JsonProperty("currency")]
        public string? Currency { get; set; }
        
        [JsonProperty("reason")]
        public string? Reason { get; set; }
        
        [JsonProperty("description")]
        public string? Description { get; set; }
        
        [JsonProperty("status")]
        public string? Status { get; set; }
        
        [JsonProperty("created")]
        public long? Created { get; set; }
    }
    
    public class CreateRefundRequest
    {
        public string PaymentId { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string? Reason { get; set; }
        public string? Description { get; set; }
    }
}

// ==================== Models/Customer.cs ====================
namespace CryptoComPay.Models
{
    public class Customer
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("email")]
        public string? Email { get; set; }
        
        [JsonProperty("ref_id")]
        public string? RefId { get; set; }
        
        [JsonProperty("created_at")]
        public long? CreatedAt { get; set; }
        
        [JsonProperty("updated_at")]
        public long? UpdatedAt { get; set; }
    }
    
    public class CreateCustomerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? RefId { get; set; }
    }
}

// ==================== Models/Product.cs ====================
namespace CryptoComPay.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("active")]
        public bool Active { get; set; } = true;
        
        [JsonProperty("description")]
        public string? Description { get; set; }
        
        [JsonProperty("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        
        [JsonProperty("pricing_plans")]
        public List<PricingPlan>? PricingPlans { get; set; }
    }
    
    public class PricingPlan
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("amount")]
        public long Amount { get; set; }
        
        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;
        
        [JsonProperty("interval")]
        public string Interval { get; set; } = "month";
        
        [JsonProperty("interval_count")]
        public int IntervalCount { get; set; } = 1;
        
        [JsonProperty("purchase_type")]
        public string PurchaseType { get; set; } = "recurring";
        
        [JsonProperty("active")]
        public bool Active { get; set; } = true;
        
        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}

// ==================== Models/Subscription.cs ====================
namespace CryptoComPay.Models
{
    public class Subscription
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; } = string.Empty;
        
        [JsonProperty("billing_cycle_anchor")]
        public long BillingCycleAnchor { get; set; }
        
        [JsonProperty("items")]
        public List<SubscriptionItem> Items { get; set; } = new();
        
        [JsonProperty("status")]
        public string? Status { get; set; }
        
        [JsonProperty("amount")]
        public long? Amount { get; set; }
        
        [JsonProperty("currency")]
        public string? Currency { get; set; }
        
        [JsonProperty("subscription_url")]
        public string? SubscriptionUrl { get; set; }
        
        [JsonProperty("return_url")]
        public string? ReturnUrl { get; set; }
    }
    
    public class SubscriptionItem
    {
        [JsonProperty("plan_id")]
        public string PlanId { get; set; } = string.Empty;
        
        [JsonProperty("quantity")]
        public int Quantity { get; set; } = 1;
    }
}

// ==================== Models/Common.cs ====================
namespace CryptoComPay.Models
{
    public class PagedResponse<T>
    {
        [JsonProperty("items")]
        public List<T>? Items { get; set; }
        
        [JsonProperty("meta")]
        public PageMeta? Meta { get; set; }
    }
    
    public class PageMeta
    {
        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }
        
        [JsonProperty("per_page")]
        public int PerPage { get; set; }
        
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }
    }
    
    public class ApiError
    {
        [JsonProperty("error")]
        public ErrorDetail? Error { get; set; }
    }
    
    public class ErrorDetail
    {
        [JsonProperty("type")]
        public string? Type { get; set; }
        
        [JsonProperty("code")]
        public string? Code { get; set; }
        
        [JsonProperty("param")]
        public string? Param { get; set; }
        
        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}

// ==================== Services/ICryptoComPayClient.cs ====================
using CryptoComPay.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoComPay.Services
{
    public interface ICryptoComPayClient
    {
        Task<Payment> CreatePaymentAsync(CreatePaymentRequest request);
        Task<Payment> GetPaymentAsync(string paymentId);
        Task<Payment> CancelPaymentAsync(string paymentId);
        Task<PagedResponse<Payment>> ListPaymentsAsync(int page = 1, int perPage = 10);
        
        Task<Refund> CreateRefundAsync(CreateRefundRequest request);
        Task<Refund> GetRefundAsync(string refundId);
        Task<List<Refund>> GetPaymentRefundsAsync(string paymentId);
        
        Task<Customer> CreateCustomerAsync(CreateCustomerRequest request);
        Task<Customer> GetCustomerAsync(string customerId);
        Task<PagedResponse<Customer>> ListCustomersAsync(int page = 1, int perPage = 10);
        
        Task<Product> CreateProductAsync(Product product);
        Task<Product> GetProductAsync(string productId);
        
        Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
        Task<Subscription> GetSubscriptionAsync(string subscriptionId);
        Task<Subscription> CancelSubscriptionAsync(string subscriptionId);
    }
}

// ==================== Services/CryptoComPayClient.cs ====================
using CryptoComPay.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CryptoComPay.Services
{
    public class CryptoComPayClient : ICryptoComPayClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private const string BaseUrl = "https://pay.crypto.com/api";
        
        public CryptoComPayClient(string secretKey)
        {
            _secretKey = secretKey;
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            
            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_secretKey}:"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
        }
        
        public async Task<Payment> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var payload = new Dictionary<string, object>
            {
                ["amount"] = request.Amount,
                ["currency"] = request.Currency
            };
            
            if (request.Description != null) payload["description"] = request.Description;
            if (request.OrderId != null) payload["order_id"] = request.OrderId;
            if (request.ReturnUrl != null) payload["return_url"] = request.ReturnUrl;
            if (request.CancelUrl != null) payload["cancel_url"] = request.CancelUrl;
            if (request.Metadata != null) payload["metadata"] = request.Metadata;
            
            return await PostAsync<Payment>("/payments", payload);
        }
        
        public async Task<Payment> GetPaymentAsync(string paymentId)
        {
            return await GetAsync<Payment>($"/payments/{paymentId}");
        }
        
        public async Task<Payment> CancelPaymentAsync(string paymentId)
        {
            return await PostAsync<Payment>($"/payments/{paymentId}/cancel", null);
        }
        
        public async Task<PagedResponse<Payment>> ListPaymentsAsync(int page = 1, int perPage = 10)
        {
            return await GetAsync<PagedResponse<Payment>>($"/payments?page={page}&per_page={perPage}");
        }
        
        public async Task<Refund> CreateRefundAsync(CreateRefundRequest request)
        {
            var payload = new Dictionary<string, object>
            {
                ["payment_id"] = request.PaymentId,
                ["amount"] = request.Amount
            };
            
            if (request.Reason != null) payload["reason"] = request.Reason;
            if (request.Description != null) payload["description"] = request.Description;
            
            return await PostAsync<Refund>("/refunds", payload);
        }
        
        public async Task<Refund> GetRefundAsync(string refundId)
        {
            return await GetAsync<Refund>($"/refunds/{refundId}");
        }
        
        public async Task<List<Refund>> GetPaymentRefundsAsync(string paymentId)
        {
            return await GetAsync<List<Refund>>($"/payments/{paymentId}/refunds");
        }
        
        public async Task<Customer> CreateCustomerAsync(CreateCustomerRequest request)
        {
            var payload = new Dictionary<string, object>
            {
                ["name"] = request.Name
            };
            
            if (request.Email != null) payload["email"] = request.Email;
            if (request.RefId != null) payload["ref_id"] = request.RefId;
            
            return await PostAsync<Customer>("/customers", payload);
        }
        
        public async Task<Customer> GetCustomerAsync(string customerId)
        {
            return await GetAsync<Customer>($"/customers/{customerId}");
        }
        
        public async Task<PagedResponse<Customer>> ListCustomersAsync(int page = 1, int perPage = 10)
        {
            return await GetAsync<PagedResponse<Customer>>($"/customers?page={page}&per_page={perPage}");
        }
        
        public async Task<Product> CreateProductAsync(Product product)
        {
            return await PostAsync<Product>("/products", product);
        }
        
        public async Task<Product> GetProductAsync(string productId)
        {
            return await GetAsync<Product>($"/products/{productId}");
        }
        
        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
        {
            return await PostAsync<Subscription>("/subscriptions", subscription);
        }
        
        public async Task<Subscription> GetSubscriptionAsync(string subscriptionId)
        {
            return await GetAsync<Subscription>($"/subscriptions/{subscriptionId}");
        }
        
        public async Task<Subscription> CancelSubscriptionAsync(string subscriptionId)
        {
            return await PostAsync<Subscription>($"/subscriptions/{subscriptionId}/cancel", null);
        }
        
        private async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Error: {response.StatusCode} - {content}");
            }
            
            return JsonConvert.DeserializeObject<T>(content)!;
        }
        
        private async Task<T> PostAsync<T>(string endpoint, object? payload)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            
            if (payload != null)
            {
                var json = JsonConvert.SerializeObject(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Error: {response.StatusCode} - {content}");
            }
            
            return JsonConvert.DeserializeObject<T>(content)!;
        }
    }
}

// ==================== Controllers/TestController.cs ====================
using CryptoComPay.Models;
using CryptoComPay.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoComPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ICryptoComPayClient _client;
        
        public TestController(ICryptoComPayClient client)
        {
            _client = client;
        }
        
        /// <summary>
        /// Test: Create a payment
        /// </summary>
        [HttpPost("payment/create")]
        public async Task<ActionResult<Payment>> TestCreatePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var payment = await _client.CreatePaymentAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Payment created successfully",
                    data = payment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Test: Get payment by ID
        /// </summary>
        [HttpGet("payment/{paymentId}")]
        public async Task<ActionResult<Payment>> TestGetPayment(string paymentId)
        {
            try
            {
                var payment = await _client.GetPaymentAsync(paymentId);
                return Ok(new
                {
                    success = true,
                    data = payment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Test: Cancel payment
        /// </summary>
        [HttpPost("payment/{paymentId}/cancel")]
        public async Task<ActionResult<Payment>> TestCancelPayment(string paymentId)
        {
            try
            {
                var payment = await _client.CancelPaymentAsync(paymentId);
                return Ok(new
                {
                    success = true,
                    message = "Payment cancelled successfully",
                    data = payment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Test: List all payments
        /// </summary>
        [HttpGet("payment/list")]
        public async Task<ActionResult<PagedResponse<Payment>>> TestListPayments([FromQuery] int page = 1, [FromQuery] int perPage = 10)
        {
            try
            {
                var payments = await _client.ListPaymentsAsync(page, perPage);
                return Ok(new
                {
                    success = true,
                    data = payments
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Test: Create a refund
        /// </summary>
        [HttpPost("refund/create")]
        public async Task<ActionResult<Refund>> TestCreateRefund([FromBody] CreateRefundRequest request)
        {
            try
            {
                var refund = await _client.CreateRefundAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Refund created successfully",
                    data = refund
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Test: Create a customer
        /// </summary>
        [HttpPost("customer/create")]
        public async Task<ActionResult<Customer>> TestCreateCustomer([FromBody] CreateCustomerRequest request)
        {
            try
            {
                var customer = await _client.CreateCustomerAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Customer created successfully",
                    data = customer
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Test: Get customer by ID
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<Customer>> TestGetCustomer(string customerId)
        {
            try
            {
                var customer = await _client.GetCustomerAsync(customerId);
                return Ok(new
                {
                    success = true,
                    data = customer
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Test: Create a product
        /// </summary>
        [HttpPost("product/create")]
        public async Task<ActionResult<Product>> TestCreateProduct([FromBody] Product product)
        {
            try
            {
                var createdProduct = await _client.CreateProductAsync(product);
                return Ok(new
                {
                    success = true,
                    message = "Product created successfully",
                    data = createdProduct
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Test: Run full integration test
        /// </summary>
        [HttpPost("run-full-test")]
        public async Task<ActionResult> RunFullIntegrationTest()
        {
            var results = new List<object>();
            
            try
            {
                // 1. Create Payment
                var paymentRequest = new CreatePaymentRequest
                {
                    Amount = 2500,
                    Currency = "USD",
                    Description = "Test Payment from API",
                    OrderId = $"TEST-{Guid.NewGuid().ToString().Substring(0, 8)}",
                    Metadata = new Dictionary<string, string>
                    {
                        ["test"] = "true",
                        ["timestamp"] = DateTimeOffset.UtcNow.ToString()
                    }
                };
                
                var payment = await _client.CreatePaymentAsync(paymentRequest);
                results.Add(new
                {
                    step = "1. Create Payment",
                    status = "✓ Success",
                    paymentId = payment.Id,
                    amount = payment.Amount,
                    currency = payment.Currency,
                    paymentUrl = payment.PaymentUrl
                });
                
                // 2. Get Payment
                var retrievedPayment = await _client.GetPaymentAsync(payment.Id!);
                results.Add(new
                {
                    step = "2. Get Payment",
                    status = "✓ Success",
                    paymentStatus = retrievedPayment.Status
                });
                
                // 3. Create Customer
                var customerRequest = new CreateCustomerRequest
                {
                    Name = "Test Customer",
                    Email = "test@example.com",
                    RefId = $"CUST-{Guid.NewGuid().ToString().Substring(0, 8)}"
                };
                
                var customer = await _client.CreateCustomerAsync(customerRequest);
                results.Add(new
                {
                    step = "3. Create Customer",
                    status = "✓ Success",
                    customerId = customer.Id,
                    name = customer.Name
                });
                
                // 4. List Payments
                var payments = await _client.ListPaymentsAsync(1, 5);
                results.Add(new
                {
                    step = "4. List Payments",
                    status = "✓ Success",
                    totalPayments = payments.Items?.Count ?? 0
                });
                
                return Ok(new
                {
                    success = true,
                    message = "Full integration test completed successfully",
                    results = results
                });
            }
            catch (Exception ex)
            {
                results.Add(new
                {
                    step = "Error",
                    status = "✗ Failed",
                    error = ex.Message
                });
                
                return BadRequest(new
                {
                    success = false,
                    message = "Integration test failed",
                    results = results
                });
            }
        }
    }
}

// ==================== Program.cs ====================
using CryptoComPay.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Crypto.com Pay API Integration", 
        Version = "v1",
        Description = "API for testing Crypto.com Pay integration"
    });
});

// Register Crypto.com Pay Client
var secretKey = builder.Configuration["CryptoComPay:SecretKey"] ?? "sk_test_YOUR_SECRET_KEY_HERE";
builder.Services.AddSingleton<ICryptoComPayClient>(new CryptoComPayClient(secretKey));

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crypto.com Pay API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// ==================== appsettings.json ====================
/*
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "CryptoComPay": {
    "SecretKey": "sk_test_YOUR_SECRET_KEY_HERE"
  }
}
*/