// ============================================================================
//  Day 02 — Abstraction vs Encapsulation  (IPaymentProcessor)
//  The Solution Architect Foundation Journey
//
//  TYPE IT YOURSELF. Don't copy. The point is the hand, not the eye.
//  No NuGet packages needed — every "provider" here is fake.
//
//  Run: drop into a console project and `dotnet run`
// ============================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Day02;

// ============================================================================
//  PART 0 — THE LEAKY VERSION (read only, do not fix in place)
//
//  TASK 1: name the three leaks in the comment slots below.
// ============================================================================

public class StripeChargeResponse            // pretend this came from the Stripe SDK
{
    public string Id { get; set; } = "";
    public string Status { get; set; } = "";
}

public interface ILeakyPaymentProcessor
{
    // LEAK #1: StripeChargeResponse: vendor leak____________________________________________________________
    // LEAK #2: amountInCents: domain leak____________________________________________________________
    // LEAK #3: stripeCustomerId: user identity leak____________________________________________________________
    Task<StripeChargeResponse> ChargeAsync(long amountInCents, string stripeCustomerId);
}


// ============================================================================
//  PART 1 — THE CLEAN ABSTRACTION
//
//  TASK 2: write these yourself. Domain vocabulary only — no vendor words.
//
//  Before you type, answer out loud:
//    (a) What does the caller actually want?
//    (b) Which details WILL differ between providers?  -> must stay inside
//    (c) Which truths hold for EVERY provider?         -> that is the shape
//        ...and "settlement is instant" is NOT one of them.
// ============================================================================

public readonly record struct Money(decimal Amount, string Currency)
{
    public override string ToString() => $"{Amount:0.00} {Currency}";
}

public sealed record PaymentRequest(
    Money Amount,
    string CustomerReference,   // OUR id, never the vendor's
    string IdempotencyKey);     // in the contract on purpose — see session §3(a)

public enum PaymentState
{
    Authorized,   // money reserved, not captured
    Settled,      // money actually moved
    Pending,      // e.g. bank transfer: will settle in ~2 days
    Failed
}

public sealed record PaymentResult(
    PaymentState State,
    string ProviderReference,   // opaque. We never parse it.
    string? FailureReason);

public interface IPaymentProcessor
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken ct = default);
    Task<PaymentResult> RefundAsync(string providerReference, Money amount, CancellationToken ct = default);
}


// ============================================================================
//  PART 2 — TWO IMPLEMENTATIONS
//
//  TASK 3: two providers that behave GENUINELY differently. That difference
//          is the whole reason the abstraction has to be honest.
// ============================================================================

public sealed class FakeCardPaymentProcessor : IPaymentProcessor
{
    private readonly HashSet<string> _seenKeys = new(StringComparer.Ordinal);

    public Task<PaymentResult> ChargeAsync(PaymentRequest r, CancellationToken ct = default)
    {
        // Idempotency, honoured. Retry must not double-charge.
        if (!_seenKeys.Add(r.IdempotencyKey))
        {
            Console.WriteLine($"  [card] duplicate key '{r.IdempotencyKey}' -> replaying, NOT charging again");
            return Task.FromResult(new PaymentResult(PaymentState.Settled, "card_replayed", null));
        }

        Console.WriteLine($"  [card] charging {r.Amount} for {r.CustomerReference}");
        return Task.FromResult(new PaymentResult(PaymentState.Settled, $"card_{Guid.NewGuid():N}", null));
    }

    public Task<PaymentResult> RefundAsync(string providerReference, Money amount, CancellationToken ct = default)
        => throw new NotImplementedException("TASK 3a: implement this yourself.");
}

public sealed class BankTransferPaymentProcessor : IPaymentProcessor
{
    // No API. A file is generated, a human bank settles it in ~2 days.
    // This class is why PaymentState.Pending exists.
    public Task<PaymentResult> ChargeAsync(PaymentRequest r, CancellationToken ct = default)
    {
        Console.WriteLine($"  [bank] queued a transfer instruction for {r.Amount}. Settles in ~2 days.");
        return Task.FromResult(new PaymentResult(PaymentState.Pending, $"batch_{DateTime.UtcNow:yyyyMMdd}_001", null));
    }

    public Task<PaymentResult> RefundAsync(string providerReference, Money amount, CancellationToken ct = default)
        => throw new NotImplementedException("TASK 3b: implement this yourself.");
}


// ============================================================================
//  PART 3 — THE CALLER
//
//  TASK 4: make InvoiceService work with BOTH, and let the compiler force you
//          to handle Pending. If ChargeAsync returned Task<bool>, today's bug
//          would be invisible. Feel that.
// ============================================================================

public sealed class Invoice
{
    // Day 1 habit: invariants stay guarded.
    // INVARIANT: Total > 0; State only moves forward.
    public Invoice(string id, decimal total, string currency, string customerRef)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id required", nameof(id));
        if (total <= 0) throw new ArgumentOutOfRangeException(nameof(total), "Invoice total must be positive.");
        Id = id; Total = total; Currency = currency; CustomerRef = customerRef;
    }

    public string Id { get; }
    public decimal Total { get; }
    public string Currency { get; }
    public string CustomerRef { get; }
    public int AttemptCount { get; private set; }
    public string Status { get; private set; } = "Draft";
    public string? ProviderReference { get; private set; }

    public void RecordAttempt() => AttemptCount++;
    public void MarkPaid(string reference)                { Status = "Paid";               ProviderReference = reference; }
    public void MarkAuthorized(string reference)          { Status = "Authorized";         ProviderReference = reference; }
    public void MarkAwaitingSettlement(string reference)  { Status = "AwaitingSettlement"; ProviderReference = reference; }
    public void MarkFailed(string? reason)                { Status = $"Failed: {reason ?? "unknown"}"; }
}

public sealed class InvoiceService
{
    private readonly IPaymentProcessor _payments;   // depends on the idea, not the vendor
    public InvoiceService(IPaymentProcessor payments) => _payments = payments;

    public async Task PayAsync(Invoice invoice, CancellationToken ct = default)
    {
        invoice.RecordAttempt();

        var result = await _payments.ChargeAsync(new PaymentRequest(
            new Money(invoice.Total, invoice.Currency),
            invoice.CustomerRef,
            IdempotencyKey: $"invoice-{invoice.Id}-attempt-{invoice.AttemptCount}"), ct);

        // switch EXPRESSION over the enum: add a state later and the compiler
        // warns here. That is the abstraction protecting you.
        Action apply = result.State switch
        {
            PaymentState.Settled    => () => invoice.MarkPaid(result.ProviderReference),
            PaymentState.Authorized => () => invoice.MarkAuthorized(result.ProviderReference),
            PaymentState.Pending    => () => invoice.MarkAwaitingSettlement(result.ProviderReference),
            PaymentState.Failed     => () => invoice.MarkFailed(result.FailureReason),
            _ => throw new NotSupportedException($"Unhandled payment state: {result.State}")
        };
        apply();
    }
}


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

        Console.WriteLine("1) Card provider (settles immediately):");
        var cardInvoice = new Invoice("INV-1001", 250.00m, "USD", "cust-42");
        var cardService = new InvoiceService(new FakeCardPaymentProcessor());
        await cardService.PayAsync(cardInvoice);
        Console.WriteLine($"  -> status: {cardInvoice.Status}  ref: {cardInvoice.ProviderReference}\n");

        Console.WriteLine("2) SAME caller, bank transfer (does NOT settle now):");
        var bankInvoice = new Invoice("INV-1002", 98000.00m, "USD", "cust-99");
        var bankService = new InvoiceService(new BankTransferPaymentProcessor());
        await bankService.PayAsync(bankInvoice);
        Console.WriteLine($"  -> status: {bankInvoice.Status}  ref: {bankInvoice.ProviderReference}");
        Console.WriteLine("     NOTE: not 'Paid'. If ChargeAsync returned bool, this invoice");
        Console.WriteLine("           would have been marked Paid two days early. 98,000 of it.\n");

        Console.WriteLine("3) Idempotency — the detail that must NOT be hidden:");
        var processor = new FakeCardPaymentProcessor();
        var retryInvoice = new Invoice("INV-1003", 75.00m, "USD", "cust-7");
        var retryService = new InvoiceService(processor);
        await retryService.PayAsync(retryInvoice);
        Console.WriteLine("  (network timed out, caller retries the SAME attempt...)");
        await processor.ChargeAsync(new PaymentRequest(
            new Money(75.00m, "USD"), "cust-7", "invoice-INV-1003-attempt-1"));
        Console.WriteLine("  -> charged once, not twice.\n");

        Console.WriteLine("Now do TASK 5: break the interface on purpose, feel it, delete it.");
    }
}
