using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    }
}