using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// A newsletter subscriber. Double opt-in: created Pending with a confirmation-token hash,
/// flips to Confirmed when the emailed link is clicked. The token hash is kept afterwards so
/// the same secret backs the one-click unsubscribe link.
/// </summary>
public class Subscriber : BaseEntity
{
    // Reuses the Email value object (validated + normalized) and its registered EmailSerializer.
    public Email Email { get; set; } = null!;

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;

    // SHA-256 hash of the raw confirm token. Only the hash is stored; the raw token lives only in the email.
    public string? ConfirmationTokenHash { get; set; }

    // Opaque capability id embedded in the newsletter's unsubscribe link (not secret; a random unguessable id).
    public string UnsubscribeToken { get; set; } = string.Empty;

    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}
