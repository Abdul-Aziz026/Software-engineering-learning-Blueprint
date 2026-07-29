
namespace Abstraction;

public sealed record PaymentResult(
    PaymentState State,
    string ProviderReference,   // opaque. We never parse it.
    string? FailureReason);

