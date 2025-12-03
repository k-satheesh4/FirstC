// CryptoComPay.csproj
/*
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>
</Project>
*/

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CryptoComPay
{
    // ========== Models ==========
    
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
    }
    
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
        
        [JsonProperty("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
    
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
        
        [JsonProperty("interval")]
        public string? Interval { get; set; }
        
        [JsonProperty("subscription_url")]
        public string? SubscriptionUrl { get; set; }
        
        [JsonProperty("return_url")]
        public string? ReturnUrl { get; set; }
        
        [JsonProperty("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
    
    public class SubscriptionItem
    {
        [JsonProperty("plan_id")]
        public string PlanId { get; set; } = string.Empty;
        
        [JsonProperty("quantity")]
        public int Quantity { get; set; } = 1;
    }
    
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
        
        [JsonProperty("next_page")]
        public int? NextPage { get; set; }
    }
    
    // ========== API Client ==========
    
    public class CryptoComPayClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private const string BaseUrl = "https://pay.crypto.com/api";
        
        public CryptoComPayClient(string secretKey, bool testMode = true)
        {
            _secretKey = secretKey;
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            
            // Set up Basic Authentication
            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_secretKey}:"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
        }
        
        // ========== Payment Methods ==========
        
        public async Task<Payment> CreatePaymentAsync(
            long amount,
            string currency,
            string? description = null,
            string? orderId = null,
            string? returnUrl = null,
            string? cancelUrl = null,
            Dictionary<string, string>? metadata = null)
        {
            var payload = new Dictionary<string, object>
            {
                ["amount"] = amount,
                ["currency"] = currency
            };
            
            if (description != null) payload["description"] = description;
            if (orderId != null) payload["order_id"] = orderId;
            if (returnUrl != null) payload["return_url"] = returnUrl;
            if (cancelUrl != null) payload["cancel_url"] = cancelUrl;
            if (metadata != null) payload["metadata"] = metadata;
            
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
        
        public async Task<PagedResponse<Payment>> ListPaymentsAsync(
            int page = 1,
            int perPage = 10,
            long? createdAtBegin = null,
            long? createdAtEnd = null)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["page"] = page.ToString(),
                ["per_page"] = perPage.ToString()
            };
            
            if (createdAtBegin.HasValue) queryParams["created_at_begin"] = createdAtBegin.Value.ToString();
            if (createdAtEnd.HasValue) queryParams["created_at_end"] = createdAtEnd.Value.ToString();
            
            return await GetAsync<PagedResponse<Payment>>($"/payments?{BuildQueryString(queryParams)}");
        }
        
        // ========== Refund Methods ==========
        
        public async Task<Refund> CreateRefundAsync(
            string paymentId,
            long amount,
            string? reason = null,
            string? description = null,
            string? idempotencyKey = null)
        {
            var payload = new Dictionary<string, object>
            {
                ["payment_id"] = paymentId,
                ["amount"] = amount
            };
            
            if (reason != null) payload["reason"] = reason;
            if (description != null) payload["description"] = description;
            
            return await PostAsync<Refund>("/refunds", payload, idempotencyKey);
        }
        
        public async Task<Refund> GetRefundAsync(string refundId)
        {
            return await GetAsync<Refund>($"/refunds/{refundId}");
        }
        
        public async Task<List<Refund>> GetPaymentRefundsAsync(string paymentId)
        {
            return await GetAsync<List<Refund>>($"/payments/{paymentId}/refunds");
        }
        
        // ========== Customer Methods ==========
        
        public async Task<Customer> CreateCustomerAsync(
            string name,
            string? email = null,
            string? refId = null)
        {
            var payload = new Dictionary<string, object>
            {
                ["name"] = name
            };
            
            if (email != null) payload["email"] = email;
            if (refId != null) payload["ref_id"] = refId;
            
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
        
        // ========== Product Methods ==========
        
        public async Task<Product> CreateProductAsync(Product product)
        {
            return await PostAsync<Product>("/products", product);
        }
        
        public async Task<Product> GetProductAsync(string productId)
        {
            return await GetAsync<Product>($"/products/{productId}");
        }
        
        public async Task<PagedResponse<Product>> ListProductsAsync(int page = 1, int perPage = 10)
        {
            return await GetAsync<PagedResponse<Product>>($"/products?page={page}&per_page={perPage}");
        }
        
        // ========== Subscription Methods ==========
        
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
        
        public async Task<PagedResponse<Subscription>> ListSubscriptionsAsync(int page = 1, int perPage = 10)
        {
            return await GetAsync<PagedResponse<Subscription>>($"/subscriptions?page={page}&per_page={perPage}");
        }
        
        // ========== Helper Methods ==========
        
        private async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content)!;
        }
        
        private async Task<T> PostAsync<T>(string endpoint, object? payload, string? idempotencyKey = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            
            if (payload != null)
            {
                var json = JsonConvert.SerializeObject(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            
            if (idempotencyKey != null)
            {
                request.Headers.Add("Idempotency-Key", idempotencyKey);
            }
            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content)!;
        }
        
        private string BuildQueryString(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        }
    }
    
    // ========== Example Usage ==========
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Initialize client with your secret key
            var client = new CryptoComPayClient("sk_test_YOUR_SECRET_KEY_HERE", testMode: true);
            
            try
            {
                // Example 1: Create a payment
                Console.WriteLine("Creating payment...");
                var payment = await client.CreatePaymentAsync(
                    amount: 2500,  // $25.00 (in cents)
                    currency: "USD",
                    description: "Test Product",
                    orderId: "ORDER-123",
                    returnUrl: "https://yoursite.com/success",
                    cancelUrl: "https://yoursite.com/cancel",
                    metadata: new Dictionary<string, string>
                    {
                        ["customer_name"] = "John Doe",
                        ["customer_email"] = "john@example.com"
                    }
                );
                
                Console.WriteLine($"Payment created: {payment.Id}");
                Console.WriteLine($"Payment URL: {payment.PaymentUrl}");
                Console.WriteLine($"Status: {payment.Status}");
                
                // Example 2: Get payment details
                Console.WriteLine("\nRetrieving payment...");
                var retrievedPayment = await client.GetPaymentAsync(payment.Id!);
                Console.WriteLine($"Retrieved payment status: {retrievedPayment.Status}");
                
                // Example 3: Create a customer
                Console.WriteLine("\nCreating customer...");
                var customer = await client.CreateCustomerAsync(
                    name: "Jane Smith",
                    email: "jane@example.com",
                    refId: "CUST-456"
                );
                Console.WriteLine($"Customer created: {customer.Id}");
                
                // Example 4: Create a product with pricing plan
                Console.WriteLine("\nCreating product...");
                var product = new Product
                {
                    Name = "Monthly Newsletter",
                    Active = true,
                    Description = "Monthly newsletter subscription",
                    Metadata = new Dictionary<string, string> { ["type"] = "subscription" },
                    PricingPlans = new List<PricingPlan>
                    {
                        new PricingPlan
                        {
                            Amount = 1000, // $10.00
                            Currency = "USD",
                            Interval = "month",
                            IntervalCount = 1,
                            PurchaseType = "recurring",
                            Description = "Monthly plan",
                            Active = true
                        }
                    }
                };
                
                var createdProduct = await client.CreateProductAsync(product);
                Console.WriteLine($"Product created: {createdProduct.Id}");
                Console.WriteLine($"Pricing plan ID: {createdProduct.PricingPlans?[0].Id}");
                
                // Example 5: Create a subscription
                Console.WriteLine("\nCreating subscription...");
                var subscription = new Subscription
                {
                    CustomerId = customer.Id!,
                    BillingCycleAnchor = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Items = new List<SubscriptionItem>
                    {
                        new SubscriptionItem
                        {
                            PlanId = createdProduct.PricingPlans![0].Id!,
                            Quantity = 1
                        }
                    },
                    ReturnUrl = "https://yoursite.com/subscription-success",
                    Metadata = new Dictionary<string, string> { ["subscription_type"] = "premium" }
                };
                
                var createdSubscription = await client.CreateSubscriptionAsync(subscription);
                Console.WriteLine($"Subscription created: {createdSubscription.Id}");
                Console.WriteLine($"Subscription URL: {createdSubscription.SubscriptionUrl}");
                
                // Example 6: List all payments
                Console.WriteLine("\nListing payments...");
                var payments = await client.ListPaymentsAsync(page: 1, perPage: 10);
                Console.WriteLine($"Found {payments.Items?.Count ?? 0} payments");
                
                // Example 7: Create a refund
                Console.WriteLine("\nCreating refund...");
                var refund = await client.CreateRefundAsync(
                    paymentId: payment.Id!,
                    amount: 1000, // Partial refund of $10.00
                    reason: "requested_by_customer",
                    description: "Customer requested partial refund"
                );
                Console.WriteLine($"Refund created: {refund.Id}");
                Console.WriteLine($"Refund status: {refund.Status}");
                
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}