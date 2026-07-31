
namespace Abstraction;

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

