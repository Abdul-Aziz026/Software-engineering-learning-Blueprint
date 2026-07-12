namespace Contracts.Messaging;

/// <summary>
/// Event: an admin published a post. The newsletter fan-out consumer reacts by loading the confirmed
/// subscribers and emitting one <see cref="SendNewsletterEmail"/> per recipient. Kept thin (just the id)
/// so the consumer reads the current post/subscriber state at processing time.
/// </summary>
public record PostPublished(string PostId);
