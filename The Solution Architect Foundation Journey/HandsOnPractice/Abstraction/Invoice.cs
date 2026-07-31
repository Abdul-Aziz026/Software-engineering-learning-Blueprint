
namespace Abstraction;

// Day 1 habit: invariants stay guarded...
// INVARIANT: Total > 0; State only moves forward...
public sealed class Invoice
{
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
    public void MarkPaid(string reference) { Status = "Paid"; ProviderReference = reference; }
    public void MarkAuthorized(string reference) { Status = "Authorized"; ProviderReference = reference; }
    public void MarkAwaitingSettlement(string reference) { Status = "AwaitingSettlement"; ProviderReference = reference; }
    public void MarkFailed(string? reason) { Status = $"Failed: {reason ?? "unknown"}"; }
}
