
namespace Abstraction;

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