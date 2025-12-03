// ============================================================================
// IN-STORE PAYMENT SYSTEM - Complete Solution
// ============================================================================
// This system includes:
// 1. POS Terminal Application (WPF/Console)
// 2. QR Code Generation for customer scanning
// 3. Real-time payment status monitoring
// 4. Receipt generation
// ============================================================================

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Drawing;
using QRCoder; // Install-Package QRCoder
using System.IO;
using System.Timers;

namespace CryptoComInStorePayment
{
    // ============================================================================
    // MODELS
    // ============================================================================
    
    public class InStorePaymentRequest
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [JsonPropertyName("onchain_allowed")]
        public bool OnchainAllowed { get; set; } = true;
    }

    public class PaymentResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("crypto_currency")]
        public string CryptoCurrency { get; set; }

        [JsonPropertyName("crypto_amount")]
        public string CryptoAmount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("payment_url")]
        public string PaymentUrl { get; set; }

        [JsonPropertyName("expired_at")]
        public long ExpiredAt { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        public DateTime CreatedDateTime => DateTimeOffset.FromUnixTimeSeconds(Created).DateTime;
        public DateTime ExpiredDateTime => DateTimeOffset.FromUnixTimeSeconds(ExpiredAt).DateTime;
    }

    public class Receipt
    {
        public string OrderId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CryptoAmount { get; set; }
        public string CryptoCurrency { get; set; }
        public string PaymentId { get; set; }
        public string Status { get; set; }
        public string StoreName { get; set; }
        public List<LineItem> Items { get; set; }
    }

    public class LineItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }

    // ============================================================================
    // CRYPTO.COM PAY SERVICE
    // ============================================================================
    
    public class CryptoComInStoreService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private const string BaseUrl = "https://pay.crypto.com/api";

        public CryptoComInStoreService(string secretKey)
        {
            _secretKey = secretKey;
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            var authValue = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{secretKey}:"));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Basic", authValue);
        }

        public async Task<PaymentResponse> CreateInStorePaymentAsync(
            decimal amount, 
            string currency, 
            string orderId,
            string description,
            string terminalId,
            string cashierName)
        {
            var request = new InStorePaymentRequest
            {
                Amount = (int)(amount * 100), // Convert to cents
                Currency = currency,
                Description = description,
                OrderId = orderId,
                OnchainAllowed = true,
                Metadata = new Dictionary<string, string>
                {
                    ["terminal_id"] = terminalId,
                    ["cashier"] = cashierName,
                    ["payment_type"] = "in_store",
                    ["timestamp"] = DateTime.UtcNow.ToString("o")
                }
            };

            var formData = new Dictionary<string, string>
            {
                ["amount"] = request.Amount.ToString(),
                ["currency"] = request.Currency,
                ["description"] = request.Description ?? "",
                ["order_id"] = request.OrderId ?? "",
                ["onchain_allowed"] = request.OnchainAllowed.ToString().ToLower()
            };

            // Add metadata
            if (request.Metadata != null)
            {
                var metadataJson = JsonSerializer.Serialize(request.Metadata);
                formData["metadata"] = metadataJson;
            }

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("/payments", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Payment creation failed: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponse>(json);
        }

        public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
        {
            var response = await _httpClient.GetAsync($"/payments/{paymentId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get payment status: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponse>(json);
        }

        public async Task<PaymentResponse> CancelPaymentAsync(string paymentId)
        {
            var response = await _httpClient.PostAsync($"/payments/{paymentId}/cancel", null);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to cancel payment: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponse>(json);
        }
    }

    // ============================================================================
    // QR CODE GENERATOR
    // ============================================================================
    
    public class QRCodeGenerator
    {
        public static byte[] GenerateQRCode(string paymentUrl, int pixelsPerModule = 10)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(paymentUrl, 
                    QRCodeGenerator.ECCLevel.Q);
                
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(pixelsPerModule);
                }
            }
        }

        public static void SaveQRCodeToFile(string paymentUrl, string filePath)
        {
            var qrBytes = GenerateQRCode(paymentUrl);
            File.WriteAllBytes(filePath, qrBytes);
        }

        public static string GenerateQRCodeBase64(string paymentUrl)
        {
            var qrBytes = GenerateQRCode(paymentUrl);
            return Convert.ToBase64String(qrBytes);
        }
    }

    // ============================================================================
    // RECEIPT PRINTER
    // ============================================================================
    
    public class ReceiptPrinter
    {
        public static string GenerateTextReceipt(Receipt receipt)
        {
            var sb = new StringBuilder();
            var width = 42;

            sb.AppendLine(new string('=', width));
            sb.AppendLine(CenterText(receipt.StoreName, width));
            sb.AppendLine(CenterText("PAYMENT RECEIPT", width));
            sb.AppendLine(new string('=', width));
            sb.AppendLine();
            
            sb.AppendLine($"Date/Time: {receipt.TransactionDate:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Order ID:  {receipt.OrderId}");
            sb.AppendLine($"Payment:   {receipt.PaymentId}");
            sb.AppendLine();
            sb.AppendLine(new string('-', width));
            
            // Items
            if (receipt.Items != null && receipt.Items.Any())
            {
                sb.AppendLine("ITEMS:");
                foreach (var item in receipt.Items)
                {
                    sb.AppendLine($"{item.Quantity}x {item.Name}");
                    sb.AppendLine($"   @ {item.UnitPrice:C} = {item.Total:C}");
                }
                sb.AppendLine(new string('-', width));
            }
            
            // Totals
            sb.AppendLine($"TOTAL:     {receipt.Amount:C} {receipt.Currency}");
            
            if (!string.IsNullOrEmpty(receipt.CryptoAmount))
            {
                sb.AppendLine($"Paid:      {receipt.CryptoAmount} {receipt.CryptoCurrency}");
            }
            
            sb.AppendLine($"Status:    {receipt.Status.ToUpper()}");
            sb.AppendLine();
            sb.AppendLine(new string('=', width));
            sb.AppendLine(CenterText("Thank you for your purchase!", width));
            sb.AppendLine(CenterText("Powered by Crypto.com Pay", width));
            sb.AppendLine(new string('=', width));

            return sb.ToString();
        }

        private static string CenterText(string text, int width)
        {
            if (text.Length >= width) return text;
            var padding = (width - text.Length) / 2;
            return new string(' ', padding) + text;
        }

        public static void PrintReceiptToFile(Receipt receipt, string filePath)
        {
            var receiptText = GenerateTextReceipt(receipt);
            File.WriteAllText(filePath, receiptText);
        }

        public static void PrintReceiptToConsole(Receipt receipt)
        {
            var receiptText = GenerateTextReceipt(receipt);
            Console.WriteLine(receiptText);
        }
    }

    // ============================================================================
    // POS TERMINAL SIMULATOR
    // ============================================================================
    
    public class POSTerminal
    {
        private readonly CryptoComInStoreService _paymentService;
        private readonly string _terminalId;
        private readonly string _storeName;
        private System.Timers.Timer _statusCheckTimer;
        private string _currentPaymentId;

        public POSTerminal(string secretKey, string terminalId, string storeName)
        {
            _paymentService = new CryptoComInStoreService(secretKey);
            _terminalId = terminalId;
            _storeName = storeName;
        }

        public async Task ProcessInStorePayment(
            List<LineItem> items,
            string cashierName,
            string currency = "USD")
        {
            try
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════╗");
                Console.WriteLine("║     CRYPTO.COM PAY - IN-STORE PAYMENT      ║");
                Console.WriteLine("╚════════════════════════════════════════════╝");
                Console.WriteLine();

                // Calculate total
                var totalAmount = items.Sum(i => i.Total);
                var orderId = $"ORD-{DateTime.Now:yyyyMMddHHmmss}";

                // Display order summary
                Console.WriteLine("ORDER SUMMARY:");
                Console.WriteLine(new string('-', 44));
                foreach (var item in items)
                {
                    Console.WriteLine($"{item.Quantity}x {item.Name,-25} ${item.Total:F2}");
                }
                Console.WriteLine(new string('-', 44));
                Console.WriteLine($"{"TOTAL:",-31} ${totalAmount:F2}");
                Console.WriteLine();

                // Create payment
                Console.WriteLine("Creating payment...");
                var payment = await _paymentService.CreateInStorePaymentAsync(
                    totalAmount,
                    currency,
                    orderId,
                    $"In-store purchase - {items.Count} items",
                    _terminalId,
                    cashierName
                );

                _currentPaymentId = payment.Id;

                Console.WriteLine($"✓ Payment created: {payment.Id}");
                Console.WriteLine($"  Expires: {payment.ExpiredDateTime:HH:mm:ss}");
                Console.WriteLine();

                // Generate and display QR code
                Console.WriteLine("Generating QR Code...");
                var qrFilePath = $"payment_qr_{orderId}.png";
                QRCodeGenerator.SaveQRCodeToFile(payment.PaymentUrl, qrFilePath);
                Console.WriteLine($"✓ QR Code saved to: {qrFilePath}");
                Console.WriteLine();

                // Display payment URL for manual entry
                Console.WriteLine("═══════════════════════════════════════════");
                Console.WriteLine("   CUSTOMER: SCAN QR CODE TO PAY");
                Console.WriteLine("═══════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine($"Amount: ${totalAmount:F2} {currency}");
                Console.WriteLine();
                Console.WriteLine("Or visit:");
                Console.WriteLine(payment.PaymentUrl);
                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("Waiting for payment...");
                Console.WriteLine("(Press 'C' to cancel)");
                Console.WriteLine();

                // Start monitoring payment status
                var paymentCompleted = await MonitorPaymentStatus(payment.Id, payment.ExpiredAt);

                if (paymentCompleted)
                {
                    // Get final payment details
                    var finalPayment = await _paymentService.GetPaymentStatusAsync(payment.Id);
                    
                    Console.Clear();
                    Console.WriteLine("╔════════════════════════════════════════════╗");
                    Console.WriteLine("║         PAYMENT SUCCESSFUL! ✓              ║");
                    Console.WriteLine("╚════════════════════════════════════════════╝");
                    Console.WriteLine();

                    // Generate receipt
                    var receipt = new Receipt
                    {
                        OrderId = orderId,
                        TransactionDate = DateTime.Now,
                        Amount = totalAmount,
                        Currency = currency,
                        CryptoAmount = finalPayment.CryptoAmount,
                        CryptoCurrency = finalPayment.CryptoCurrency,
                        PaymentId = finalPayment.Id,
                        Status = "PAID",
                        StoreName = _storeName,
                        Items = items
                    };

                    // Print receipt
                    ReceiptPrinter.PrintReceiptToConsole(receipt);
                    
                    var receiptFilePath = $"receipt_{orderId}.txt";
                    ReceiptPrinter.PrintReceiptToFile(receipt, receiptFilePath);
                    Console.WriteLine($"\n✓ Receipt saved to: {receiptFilePath}");
                }
                else
                {
                    Console.WriteLine("\n✗ Payment was not completed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }
            finally
            {
                _statusCheckTimer?.Stop();
                _statusCheckTimer?.Dispose();
            }
        }

        private async Task<bool> MonitorPaymentStatus(string paymentId, long expiresAt)
        {
            var expiryTime = DateTimeOffset.FromUnixTimeSeconds(expiresAt);
            var tcs = new TaskCompletionSource<bool>();

            // Set up timer to check status every 2 seconds
            _statusCheckTimer = new System.Timers.Timer(2000);
            _statusCheckTimer.Elapsed += async (sender, e) =>
            {
                try
                {
                    // Check for manual cancellation
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.C)
                        {
                            Console.WriteLine("\nCancelling payment...");
                            await _paymentService.CancelPaymentAsync(paymentId);
                            _statusCheckTimer.Stop();
                            tcs.TrySetResult(false);
                            return;
                        }
                    }

                    // Check if expired
                    if (DateTime.UtcNow > expiryTime)
                    {
                        Console.WriteLine("\n✗ Payment expired.");
                        _statusCheckTimer.Stop();
                        tcs.TrySetResult(false);
                        return;
                    }

                    // Check payment status
                    var status = await _paymentService.GetPaymentStatusAsync(paymentId);
                    
                    if (status.Status == "succeeded")
                    {
                        _statusCheckTimer.Stop();
                        tcs.TrySetResult(true);
                    }
                    else if (status.Status == "cancelled")
                    {
                        _statusCheckTimer.Stop();
                        tcs.TrySetResult(false);
                    }
                    else
                    {
                        // Still pending, show countdown
                        var remaining = expiryTime - DateTime.UtcNow;
                        Console.Write($"\rTime remaining: {remaining.Minutes:D2}:{remaining.Seconds:D2}  ");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError checking status: {ex.Message}");
                }
            };

            _statusCheckTimer.Start();
            return await tcs.Task;
        }
    }

    // ============================================================================
    // MAIN PROGRAM - POS TERMINAL APPLICATION
    // ============================================================================
    
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Configuration
            const string SECRET_KEY = "sk_test_YOUR_SECRET_KEY_HERE";
            const string TERMINAL_ID = "TERMINAL-001";
            const string STORE_NAME = "Crypto Store #1";
            const string CASHIER_NAME = "John Doe";

            var terminal = new POSTerminal(SECRET_KEY, TERMINAL_ID, STORE_NAME);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════╗");
                Console.WriteLine("║         POS TERMINAL - MAIN MENU           ║");
                Console.WriteLine("╚════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("1. New Transaction");
                Console.WriteLine("2. Sample Transaction (Demo)");
                Console.WriteLine("3. Exit");
                Console.WriteLine();
                Console.Write("Select option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ProcessNewTransaction(terminal, CASHIER_NAME);
                        break;
                    case "2":
                        await ProcessSampleTransaction(terminal, CASHIER_NAME);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static async Task ProcessNewTransaction(POSTerminal terminal, string cashierName)
        {
            var items = new List<LineItem>();
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("═══════════════════════════════════════════");
                Console.WriteLine("         NEW TRANSACTION");
                Console.WriteLine("═══════════════════════════════════════════");
                Console.WriteLine();
                
                if (items.Any())
                {
                    Console.WriteLine("Current Items:");
                    foreach (var item in items)
                    {
                        Console.WriteLine($"  {item.Quantity}x {item.Name} @ ${item.UnitPrice:F2} = ${item.Total:F2}");
                    }
                    Console.WriteLine($"\nSubtotal: ${items.Sum(i => i.Total):F2}");
                    Console.WriteLine();
                }

                Console.WriteLine("1. Add Item");
                Console.WriteLine("2. Process Payment");
                Console.WriteLine("3. Cancel");
                Console.Write("\nSelect option: ");

                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Item name: ");
                    var name = Console.ReadLine();
                    Console.Write("Quantity: ");
                    var qty = int.Parse(Console.ReadLine() ?? "1");
                    Console.Write("Unit price: $");
                    var price = decimal.Parse(Console.ReadLine() ?? "0");

                    items.Add(new LineItem
                    {
                        Name = name,
                        Quantity = qty,
                        UnitPrice = price
                    });
                }
                else if (choice == "2" && items.Any())
                {
                    await terminal.ProcessInStorePayment(items, cashierName);
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    break;
                }
                else if (choice == "3")
                {
                    break;
                }
            }
        }

        static async Task ProcessSampleTransaction(POSTerminal terminal, string cashierName)
        {
            var sampleItems = new List<LineItem>
            {
                new LineItem { Name = "Coffee", Quantity = 2, UnitPrice = 4.50m },
                new LineItem { Name = "Croissant", Quantity = 1, UnitPrice = 3.25m },
                new LineItem { Name = "Orange Juice", Quantity = 1, UnitPrice = 5.00m }
            };

            await terminal.ProcessInStorePayment(sampleItems, cashierName);
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}

// ============================================================================
// INSTALLATION & SETUP INSTRUCTIONS
// ============================================================================
/*

1. CREATE NEW PROJECT:
   dotnet new console -n CryptoComInStorePayment
   cd CryptoComInStorePayment

2. INSTALL REQUIRED PACKAGES:
   dotnet add package QRCoder
   dotnet add package System.Drawing.Common

3. UPDATE SECRET KEY:
   - Get your secret key from: https://merchant.crypto.com
   - Replace "sk_test_YOUR_SECRET_KEY_HERE" in the code

4. RUN THE APPLICATION:
   dotnet run

5. TESTING:
   - Select option 2 for a demo transaction
   - A QR code PNG file will be generated
   - Scan with Crypto.com App to complete payment
   - Receipt will be printed to console and saved to file

6. PRODUCTION DEPLOYMENT:
   - Use live mode secret key (sk_live_...)
   - Connect to actual receipt printer hardware
   - Implement proper error logging
   - Add database for transaction history
   - Set up webhook listener for payment notifications

*/