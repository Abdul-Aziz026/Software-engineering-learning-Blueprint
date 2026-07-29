
namespace Abstraction;

public enum PaymentState
{
    Authorized,   // money reserved, not captured
    Settled,      // money actually moved
    Pending,      // e.g. bank transfer: will settle in ~2 days
    Failed
}
