
namespace Abstraction;

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

