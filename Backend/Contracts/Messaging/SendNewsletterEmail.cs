namespace Contracts.Messaging;

/// <summary>
/// Command: deliver the newsletter for one published post to one subscriber. One message per recipient,
/// so each delivery is retried / dead-lettered independently and a redelivery can't re-send the whole batch.
/// </summary>
public record SendNewsletterEmail(
    string ToEmail,
    string Title,
    string Summary,
    string PostLink,
    string UnsubscribeLink);
