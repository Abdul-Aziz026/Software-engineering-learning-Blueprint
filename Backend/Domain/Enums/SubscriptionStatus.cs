namespace Domain.Enums;

public enum SubscriptionStatus
{
    // Signed up but has not clicked the confirmation link yet (double opt-in).
    Pending,
    // Confirmed their email; receives the newsletter.
    Confirmed,
    // Opted out; no longer emailed.
    Unsubscribed
}
