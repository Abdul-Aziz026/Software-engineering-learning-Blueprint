
namespace Abstraction;

public interface IGoodPaymentProcessor
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken ct = default);
    Task<PaymentResult> RefundAsync(string providerReference, Money amount, CancellationToken ct = default);
}

public class PaymentResult
{
}

public sealed class FakeCardPaymentProcessor : IGoodPaymentProcessor
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
    {
        throw new NotImplementedException("TASK 3a: implement this yourself.");
    }
}

public sealed class BankTransferPaymentProcessor : IGoodPaymentProcessor
{
    // This class is why PaymentState.Pending exists.
    public Task<PaymentResult> ChargeAsync(PaymentRequest r, CancellationToken ct = default)
    {
        Console.WriteLine($"  [bank] queued a transfer instruction for {r.Amount}. Settles in ~2 days.");
        return Task.FromResult(new PaymentResult(PaymentState.Pending, $"batch_{DateTime.UtcNow:yyyyMMdd}_001", null));
    }

    public Task<PaymentResult> RefundAsync(string providerReference, Money amount, CancellationToken ct = default)
    {
        throw new NotImplementedException("TASK 3b: implement this yourself.");
    }
}

public record struct Money(decimal Amount, string Currency);

public record PaymentRequest(string IdempotencyKey, Money Amount, string CustomerReference);