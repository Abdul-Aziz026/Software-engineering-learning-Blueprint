
namespace Abstraction;

public sealed class PaymentRequest
{
    public Money Amount { get; }
    public string CustomerReference { get; } // OUR id, never the vendor's
    public string IdempotencyKey { get; } // in the contract on purpose

    public PaymentRequest(Money amount, string customerReference, string idempotencyKey)
    {
        Amount = amount;
        CustomerReference = customerReference;
        this.IdempotencyKey = idempotencyKey;
    }

    public PaymentRequest()
    {
        
    }
}
