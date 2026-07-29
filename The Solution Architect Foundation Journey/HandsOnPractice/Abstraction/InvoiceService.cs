
namespace Abstraction;

// ============================================================================
//  PART 3 — THE CALLER
//
//  TASK 4: make InvoiceService work with BOTH processors, and let the compiler
//          force you to handle Pending. If ChargeAsync returned Task<bool>,
//          today's bug would be invisible. Feel that.
// ============================================================================

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
            idempotencyKey: $"invoice-{invoice.Id}-attempt-{invoice.AttemptCount}"), ct);

        // switch EXPRESSION over the enum: add a state later and the compiler
        // warns here. That is the abstraction protecting you.
        Action apply = result.State switch
        {
            PaymentState.Settled => () => invoice.MarkPaid(result.ProviderReference),
            PaymentState.Authorized => () => invoice.MarkAuthorized(result.ProviderReference),
            PaymentState.Pending => () => invoice.MarkAwaitingSettlement(result.ProviderReference),
            PaymentState.Failed => () => invoice.MarkFailed(result.FailureReason),
            _ => throw new NotSupportedException($"Unhandled payment state: {result.State}")
        };
        apply();
    }
}
