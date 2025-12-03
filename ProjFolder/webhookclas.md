using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoComPay.Webhooks
{
    // Base webhook event
    public class WebhookEvent
    {
        [Required]
        public string EventType { get; set; }
        
        [Required]
        public string EventId { get; set; }
        
        [Required]
        public DateTime EventTime { get; set; }
        
        [Required]
        public string Signature { get; set; }
    }

    // Payment webhook events
    public class PaymentWebhook : WebhookEvent
    {
        [Required]
        public PaymentData Data { get; set; }
    }

    public class PaymentData
    {
        [Required]
        public string PaymentId { get; set; }
        
        public string OrderId { get; set; }
        public string MerchantId { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string CryptoCurrency { get; set; }
        public decimal CryptoAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime ExpiryTime { get; set; }
        public string TransactionHash { get; set; }
        public string Network { get; set; }
        public PaymentMetadata Metadata { get; set; }
    }

    public class PaymentMetadata
    {
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public Dictionary<string, object> CustomData { get; set; }
    }

    // Checkout payment flow
    public class CheckoutPaymentWebhook : WebhookEvent
    {
        public CheckoutPaymentData Data { get; set; }
    }

    public class CheckoutPaymentData
    {
        public string CheckoutId { get; set; }
        public string PaymentId { get; set; }
        public string Status { get; set; }
        public string RedirectUrl { get; set; }
        public CheckoutItems Items { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
    }

    public class CheckoutItems
    {
        public List<CheckoutItem> Items { get; set; }
    }

    public class CheckoutItem
    {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // Subscription flow
    public class SubscriptionWebhook : WebhookEvent
    {
        public SubscriptionData Data { get; set; }
    }

    public class SubscriptionData
    {
        public string SubscriptionId { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public string PlanId { get; set; }
        public string BillingInterval { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public SubscriptionMetadata Metadata { get; set; }
    }

    public class SubscriptionMetadata
    {
        public string TrialPeriod { get; set; }
        public Dictionary<string, object> CustomFields { get; set; }
    }

    // Refund webhook
    public class RefundWebhook : WebhookEvent
    {
        public RefundData Data { get; set; }
    }

    public class RefundData
    {
        public string RefundId { get; set; }
        public string PaymentId { get; set; }
        public string Status { get; set; }
        public decimal RefundAmount { get; set; }
        public string Currency { get; set; }
        public decimal CryptoRefundAmount { get; set; }
        public string CryptoCurrency { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }

    // Unresolved payment
    public class UnresolvedPaymentWebhook : WebhookEvent
    {
        public UnresolvedPaymentData Data { get; set; }
    }

    public class UnresolvedPaymentData
    {
        public string PaymentId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public string Currency { get; set; }
        public bool IsUnderpayment { get; set; }
        public DateTime DetectedAt { get; set; }
    }

    // Underpayment threshold
    public class UnderpaymentWebhook : WebhookEvent
    {
        public UnderpaymentData Data { get; set; }
    }

    public class UnderpaymentData
    {
        public string PaymentId { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal Difference { get; set; }
        public decimal DifferencePercentage { get; set; }
        public string Currency { get; set; }
        public bool WithinThreshold { get; set; }
        public string Action { get; set; }
    }

    // Pay Rewards
    public class RewardWebhook : WebhookEvent
    {
        public RewardData Data { get; set; }
    }

    public class RewardData
    {
        public string RewardId { get; set; }
        public string PaymentId { get; set; }
        public string CustomerId { get; set; }
        public decimal RewardAmount { get; set; }
        public string RewardCurrency { get; set; }
        public string RewardType { get; set; }
        public DateTime EarnedAt { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public string Status { get; set; }
    }

    // Payout webhook
    public class PayoutWebhook : WebhookEvent
    {
        public PayoutData Data { get; set; }
    }

    public class PayoutData
    {
        public string PayoutId { get; set; }
        public string MerchantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Destination { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<PayoutTransaction> Transactions { get; set; }
    }

    public class PayoutTransaction
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionHash { get; set; }
    }

    // POS (Point of Sale) Webhooks
    public class POSWebhook : WebhookEvent
    {
        public POSData Data { get; set; }
    }

    public class POSData
    {
        public string POSId { get; set; }
        public string TerminalId { get; set; }
        public string MerchantId { get; set; }
        public string LocationId { get; set; }
        public string Status { get; set; }
        public POSTransaction Transaction { get; set; }
        public POSDevice Device { get; set; }
    }

    public class POSTransaction
    {
        public string TransactionId { get; set; }
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CryptoCurrency { get; set; }
        public decimal CryptoAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string ReceiptNumber { get; set; }
        public string CashierName { get; set; }
        public List<POSItem> Items { get; set; }
    }

    public class POSItem
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
    }

    public class POSDevice
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string SoftwareVersion { get; set; }
        public string IpAddress { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSyncAt { get; set; }
    }

    // POS Terminal Registration
    public class POSTerminalWebhook : WebhookEvent
    {
        public POSTerminalData Data { get; set; }
    }

    public class POSTerminalData
    {
        public string TerminalId { get; set; }
        public string MerchantId { get; set; }
        public string LocationId { get; set; }
        public string TerminalName { get; set; }
        public string Status { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? ActivatedAt { get; set; }
        public Dictionary<string, object> Configuration { get; set; }
    }

    // Cryptocurrency info
    public class CryptocurrencyInfo
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Network { get; set; }
        public bool IsEnabled { get; set; }
        public decimal MinimumAmount { get; set; }
        public int Decimals { get; set; }
        public List<string> SupportedNetworks { get; set; }
    }

    // Payment currencies
    public class PaymentCurrency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public bool IsSupported { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal MaximumAmount { get; set; }
    }

    // Merchant acquirer
    public class MerchantAcquirerWebhook : WebhookEvent
    {
        public MerchantAcquirerData Data { get; set; }
    }

    public class MerchantAcquirerData
    {
        public string AcquirerId { get; set; }
        public string MerchantId { get; set; }
        public string Status { get; set; }
        public string AcquirerType { get; set; }
        public DateTime OnboardedAt { get; set; }
        public Dictionary<string, object> Configuration { get; set; }
    }

    // Fee structure
    public class FeeWebhook : WebhookEvent
    {
        public FeeData Data { get; set; }
    }

    public class FeeData
    {
        public string PaymentId { get; set; }
        public decimal TransactionFee { get; set; }
        public decimal NetworkFee { get; set; }
        public decimal TotalFee { get; set; }
        public string FeeCurrency { get; set; }
        public decimal FeePercentage { get; set; }
        public DateTime CalculatedAt { get; set; }
    }

    // Catalogue item
    public class CatalogueWebhook : WebhookEvent
    {
        public CatalogueData Data { get; set; }
    }

    public class CatalogueData
    {
        public string ItemId { get; set; }
        public string MerchantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
    }

    // Payment method
    public class PaymentMethodWebhook : WebhookEvent
    {
        public PaymentMethodData Data { get; set; }
    }

    public class PaymentMethodData
    {
        public string MethodId { get; set; }
        public string Type { get; set; }
        public string Provider { get; set; }
        public bool IsEnabled { get; set; }
        public List<string> SupportedCurrencies { get; set; }
        public List<string> SupportedCryptocurrencies { get; set; }
        public Dictionary<string, object> Configuration { get; set; }
    }

    // Webhook signature verification helper
    public static class WebhookSignatureValidator
    {
        public static bool ValidateSignature(string payload, string signature, string secret)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(
                System.Text.Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
                var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return computedSignature == signature.ToLower();
            }
        }
    }

    // Webhook event types enum
    public static class WebhookEventTypes
    {
        public const string PaymentCreated = "payment.created";
        public const string PaymentSucceeded = "payment.succeeded";
        public const string PaymentFailed = "payment.failed";
        public const string PaymentExpired = "payment.expired";
        public const string PaymentCanceled = "payment.canceled";
        public const string RefundCreated = "refund.created";
        public const string RefundSucceeded = "refund.succeeded";
        public const string RefundFailed = "refund.failed";
        public const string SubscriptionCreated = "subscription.created";
        public const string SubscriptionUpdated = "subscription.updated";
        public const string SubscriptionCanceled = "subscription.canceled";
        public const string PayoutInitiated = "payout.initiated";
        public const string PayoutCompleted = "payout.completed";
        public const string PayoutFailed = "payout.failed";
        public const string PaymentUnderpaid = "payment.underpaid";
        public const string PaymentUnresolved = "payment.unresolved";
        public const string RewardEarned = "reward.earned";
        public const string CatalogueItemUpdated = "catalogue.item.updated";
        public const string POSTransactionCreated = "pos.transaction.created";
        public const string POSTransactionCompleted = "pos.transaction.completed";
        public const string POSTransactionFailed = "pos.transaction.failed";
        public const string POSTerminalRegistered = "pos.terminal.registered";
        public const string POSTerminalActivated = "pos.terminal.activated";
        public const string POSDeviceOnline = "pos.device.online";
        public const string POSDeviceOffline = "pos.device.offline";
    }

    // ============================================
    // WEBHOOK HANDLER EXAMPLES
    // ============================================

    public class WebhookHandler
    {
        private readonly string _webhookSecret;

        public WebhookHandler(string webhookSecret)
        {
            _webhookSecret = webhookSecret;
        }

        // Example: Handle incoming webhook
        public async Task<bool> HandleWebhook(string payload, string signature)
        {
            // Validate signature
            if (!WebhookSignatureValidator.ValidateSignature(payload, signature, _webhookSecret))
            {
                throw new UnauthorizedAccessException("Invalid webhook signature");
            }

            // Parse base event to get event type
            var baseEvent = JsonSerializer.Deserialize<WebhookEvent>(payload);

            // Route to appropriate handler based on event type
            return baseEvent.EventType switch
            {
                WebhookEventTypes.PaymentSucceeded => await HandlePaymentSucceeded(payload),
                WebhookEventTypes.RefundCreated => await HandleRefundCreated(payload),
                WebhookEventTypes.SubscriptionCreated => await HandleSubscriptionCreated(payload),
                WebhookEventTypes.POSTransactionCompleted => await HandlePOSTransaction(payload),
                _ => await HandleUnknownEvent(baseEvent.EventType)
            };
        }

        private async Task<bool> HandlePaymentSucceeded(string payload)
        {
            var webhook = JsonSerializer.Deserialize<PaymentWebhook>(payload);
            Console.WriteLine($"Payment succeeded: {webhook.Data.PaymentId}");
            Console.WriteLine($"Amount: {webhook.Data.Amount} {webhook.Data.Currency}");
            // Process payment success logic
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> HandleRefundCreated(string payload)
        {
            var webhook = JsonSerializer.Deserialize<RefundWebhook>(payload);
            Console.WriteLine($"Refund created: {webhook.Data.RefundId}");
            // Process refund logic
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> HandleSubscriptionCreated(string payload)
        {
            var webhook = JsonSerializer.Deserialize<SubscriptionWebhook>(payload);
            Console.WriteLine($"Subscription created: {webhook.Data.SubscriptionId}");
            // Process subscription logic
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> HandlePOSTransaction(string payload)
        {
            var webhook = JsonSerializer.Deserialize<POSWebhook>(payload);
            Console.WriteLine($"POS Transaction: {webhook.Data.Transaction.TransactionId}");
            Console.WriteLine($"Terminal: {webhook.Data.TerminalId}");
            // Process POS transaction logic
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> HandleUnknownEvent(string eventType)
        {
            Console.WriteLine($"Unknown event type: {eventType}");
            await Task.CompletedTask;
            return false;
        }
    }

    // ============================================
    // TESTING UTILITIES
    // ============================================

    public class WebhookTestUtilities
    {
        // Generate test webhook payload
        public static string GenerateTestPaymentWebhook()
        {
            var webhook = new PaymentWebhook
            {
                EventType = WebhookEventTypes.PaymentSucceeded,
                EventId = Guid.NewGuid().ToString(),
                EventTime = DateTime.UtcNow,
                Signature = "test_signature",
                Data = new PaymentData
                {
                    PaymentId = "pay_" + Guid.NewGuid().ToString("N").Substring(0, 16),
                    OrderId = "order_" + Guid.NewGuid().ToString("N").Substring(0, 12),
                    MerchantId = "merchant_123",
                    Status = "succeeded",
                    Currency = "USD",
                    Amount = 99.99m,
                    CryptoCurrency = "BTC",
                    CryptoAmount = 0.0025m,
                    PaymentMethod = "crypto",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    CompletedAt = DateTime.UtcNow,
                    ExpiryTime = DateTime.UtcNow.AddHours(1),
                    TransactionHash = "0x" + Guid.NewGuid().ToString("N"),
                    Network = "bitcoin",
                    Metadata = new PaymentMetadata
                    {
                        CustomerId = "cust_12345",
                        CustomerEmail = "customer@example.com"
                    }
                }
            };

            return JsonSerializer.Serialize(webhook, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        public static string GenerateTestPOSWebhook()
        {
            var webhook = new POSWebhook
            {
                EventType = WebhookEventTypes.POSTransactionCompleted,
                EventId = Guid.NewGuid().ToString(),
                EventTime = DateTime.UtcNow,
                Signature = "test_signature",
                Data = new POSData
                {
                    POSId = "pos_" + Guid.NewGuid().ToString("N").Substring(0, 12),
                    TerminalId = "terminal_001",
                    MerchantId = "merchant_123",
                    LocationId = "location_store_01",
                    Status = "completed",
                    Transaction = new POSTransaction
                    {
                        TransactionId = "txn_" + Guid.NewGuid().ToString("N").Substring(0, 16),
                        PaymentId = "pay_" + Guid.NewGuid().ToString("N").Substring(0, 16),
                        Amount = 156.78m,
                        Currency = "USD",
                        CryptoCurrency = "USDC",
                        CryptoAmount = 156.78m,
                        PaymentMethod = "crypto",
                        Status = "completed",
                        InitiatedAt = DateTime.UtcNow.AddMinutes(-2),
                        CompletedAt = DateTime.UtcNow,
                        ReceiptNumber = "RCP-2024-001234",
                        CashierName = "John Doe",
                        Items = new List<POSItem>
                        {
                            new POSItem
                            {
                                Sku = "PROD-001",
                                Name = "Premium Coffee",
                                Quantity = 2,
                                UnitPrice = 4.99m,
                                TotalPrice = 9.98m,
                                TaxAmount = 0.80m,
                                DiscountAmount = 0m
                            },
                            new POSItem
                            {
                                Sku = "PROD-002",
                                Name = "Chocolate Croissant",
                                Quantity = 3,
                                UnitPrice = 3.50m,
                                TotalPrice = 10.50m,
                                TaxAmount = 0.84m,
                                DiscountAmount = 0.50m
                            }
                        }
                    },
                    Device = new POSDevice
                    {
                        DeviceId = "device_12345",
                        DeviceName = "Store Terminal 1",
                        DeviceType = "countertop",
                        SoftwareVersion = "1.5.2",
                        IpAddress = "192.168.1.100",
                        IsOnline = true,
                        LastSyncAt = DateTime.UtcNow
                    }
                }
            };

            return JsonSerializer.Serialize(webhook, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        public static string GenerateTestRefundWebhook()
        {
            var webhook = new RefundWebhook
            {
                EventType = WebhookEventTypes.RefundSucceeded,
                EventId = Guid.NewGuid().ToString(),
                EventTime = DateTime.UtcNow,
                Signature = "test_signature",
                Data = new RefundData
                {
                    RefundId = "ref_" + Guid.NewGuid().ToString("N").Substring(0, 16),
                    PaymentId = "pay_" + Guid.NewGuid().ToString("N").Substring(0, 16),
                    Status = "succeeded",
                    RefundAmount = 49.99m,
                    Currency = "USD",
                    CryptoRefundAmount = 0.00125m,
                    CryptoCurrency = "BTC",
                    Reason = "Customer request",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                    ProcessedAt = DateTime.UtcNow
                }
            };

            return JsonSerializer.Serialize(webhook, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        // Generate test signature
        public static string GenerateTestSignature(string payload, string secret)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(
                System.Text.Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        // Validate webhook structure
        public static bool ValidateWebhookStructure<T>(string payload) where T : WebhookEvent
        {
            try
            {
                var webhook = JsonSerializer.Deserialize<T>(payload);
                return webhook != null && 
                       !string.IsNullOrEmpty(webhook.EventType) && 
                       !string.IsNullOrEmpty(webhook.EventId);
            }
            catch
            {
                return false;
            }
        }
    }

    // ============================================
    // UNIT TEST EXAMPLES (xUnit style)
    // ============================================

    public class WebhookTests
    {
        private const string TestSecret = "test_webhook_secret_key_12345";

        public void TestPaymentWebhookDeserialization()
        {
            var payload = WebhookTestUtilities.GenerateTestPaymentWebhook();
            var isValid = WebhookTestUtilities.ValidateWebhookStructure<PaymentWebhook>(payload);
            
            Console.WriteLine($"Payment webhook validation: {(isValid ? "PASSED" : "FAILED")}");
            
            if (isValid)
            {
                var webhook = JsonSerializer.Deserialize<PaymentWebhook>(payload);
                Console.WriteLine($"Payment ID: {webhook.Data.PaymentId}");
                Console.WriteLine($"Amount: {webhook.Data.Amount} {webhook.Data.Currency}");
            }
        }

        public void TestPOSWebhookDeserialization()
        {
            var payload = WebhookTestUtilities.GenerateTestPOSWebhook();
            var isValid = WebhookTestUtilities.ValidateWebhookStructure<POSWebhook>(payload);
            
            Console.WriteLine($"POS webhook validation: {(isValid ? "PASSED" : "FAILED")}");
            
            if (isValid)
            {
                var webhook = JsonSerializer.Deserialize<POSWebhook>(payload);
                Console.WriteLine($"Terminal ID: {webhook.Data.TerminalId}");
                Console.WriteLine($"Transaction ID: {webhook.Data.Transaction.TransactionId}");
                Console.WriteLine($"Items count: {webhook.Data.Transaction.Items.Count}");
            }
        }

        public void TestSignatureValidation()
        {
            var payload = WebhookTestUtilities.GenerateTestPaymentWebhook();
            var signature = WebhookTestUtilities.GenerateTestSignature(payload, TestSecret);
            var isValid = WebhookSignatureValidator.ValidateSignature(payload, signature, TestSecret);
            
            Console.WriteLine($"Signature validation: {(isValid ? "PASSED" : "FAILED")}");
        }

        public void TestInvalidSignature()
        {
            var payload = WebhookTestUtilities.GenerateTestPaymentWebhook();
            var invalidSignature = "invalid_signature_12345";
            var isValid = WebhookSignatureValidator.ValidateSignature(payload, invalidSignature, TestSecret);
            
            Console.WriteLine($"Invalid signature rejection: {(!isValid ? "PASSED" : "FAILED")}");
        }

        public async Task TestWebhookHandler()
        {
            var handler = new WebhookHandler(TestSecret);
            var payload = WebhookTestUtilities.GenerateTestPaymentWebhook();
            var signature = WebhookTestUtilities.GenerateTestSignature(payload, TestSecret);
            
            try
            {
                var result = await handler.HandleWebhook(payload, signature);
                Console.WriteLine($"Webhook handler test: {(result ? "PASSED" : "FAILED")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook handler test FAILED: {ex.Message}");
            }
        }

        // Run all tests
        public async Task RunAllTests()
        {
            Console.WriteLine("=== Running Crypto.com Pay Webhook Tests ===\n");
            
            TestPaymentWebhookDeserialization();
            Console.WriteLine();
            
            TestPOSWebhookDeserialization();
            Console.WriteLine();
            
            TestSignatureValidation();
            Console.WriteLine();
            
            TestInvalidSignature();
            Console.WriteLine();
            
            await TestWebhookHandler();
            Console.WriteLine();
            
            Console.WriteLine("=== All Tests Completed ===");
        }
    }
}