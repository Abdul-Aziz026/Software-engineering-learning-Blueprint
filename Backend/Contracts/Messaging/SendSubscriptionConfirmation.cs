namespace Contracts.Messaging;

/// <summary>Command: send a newsletter subscription confirmation ("double opt-in") email.</summary>
public record SendSubscriptionConfirmation(
    string ToEmail,
    string ConfirmLink);
