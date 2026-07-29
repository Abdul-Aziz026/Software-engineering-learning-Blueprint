
namespace Abstraction;

public interface IPaymentProcessor
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken ct = default);
    Task<PaymentResult> RefundAsync(string providerReference, Money amount, CancellationToken ct = default);
}