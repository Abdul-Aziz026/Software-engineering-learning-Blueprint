
namespace Abstraction;

// ============================================================================
//  PART 4 — BREAK IT ON PURPOSE (TASK 5)
//
//  Add this to IPaymentProcessor:
//
//      Task<StripeCustomer> GetStripeCustomerAsync(string id);
//
//  Now implement it on BankTransferPaymentProcessor. There is no Stripe
//  customer. Your only options are: throw, or return a lie.
//
//  Write down which one you reached for and why it feels wrong:
//    -> ____________________________________________________________
//
//  That pain has two names you will meet later:
//    Day 12 — LSP  (a subtype that cannot honour the contract)
//    Day 15 — ISP  (an interface forcing clients to depend on what they
//                   do not use)
//
//  Then DELETE the method. That deletion is the lesson.
// ============================================================================

public static class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=== Day 02 — Abstraction vs Encapsulation ===\n");

        await CardSettlesImmediatelyAsync();
        await BankTransferDoesNotSettleNowAsync();
        await RetryIsIdempotentAsync();

        Console.WriteLine("Now do TASK 5: break the interface on purpose, feel it, delete it.");
    }

    private static async Task CardSettlesImmediatelyAsync()
    {
        Console.WriteLine("1) Card provider (settles immediately):");

        var invoice = new Invoice("INV-1001", 250.00m, "USD", "cust-42");
        var service = new InvoiceService(new FakeCardPaymentProcessor());
        await service.PayAsync(invoice);

        Console.WriteLine($"  -> status: {invoice.Status}  ref: {invoice.ProviderReference}\n");
    }

    private static async Task BankTransferDoesNotSettleNowAsync()
    {
        Console.WriteLine("2) SAME caller, bank transfer (does NOT settle now):");

        var invoice = new Invoice("INV-1002", 98000.00m, "USD", "cust-99");
        var service = new InvoiceService(new BankTransferPaymentProcessor());
        await service.PayAsync(invoice);

        Console.WriteLine($"  -> status: {invoice.Status}  ref: {invoice.ProviderReference}");
        Console.WriteLine("     NOTE: not 'Paid'. If ChargeAsync returned bool, this invoice");
        Console.WriteLine("           would have been marked Paid two days early. 98,000 of it.\n");
    }

    private static async Task RetryIsIdempotentAsync()
    {
        Console.WriteLine("3) Idempotency — the detail that must NOT be hidden:");

        var processor = new FakeCardPaymentProcessor();
        var invoice = new Invoice("INV-1003", 75.00m, "USD", "cust-7");
        var service = new InvoiceService(processor);
        await service.PayAsync(invoice);

        Console.WriteLine("  (network timed out, caller retries the SAME attempt...)");

        // Same idempotency key as the attempt above — the processor must not charge twice.
        await processor.ChargeAsync(
            new PaymentRequest(
                new Money(75.00m, "USD"), 
                "cust-7", 
                $"invoice-{invoice.Id}-attempt-{invoice.AttemptCount}"));

        Console.WriteLine("  -> charged once, not twice.\n");
    }
}
