using Domain.Entities;

namespace Application.Features.Subscribers.DTOs;

public class SubscriberDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubscribedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }

    public static SubscriberDto FromEntity(Subscriber s) => new()
    {
        Id = s.Id,
        Email = s.Email.Value,
        Status = s.Status.ToString(),
        SubscribedAt = s.SubscribedAt,
        ConfirmedAt = s.ConfirmedAt
    };
}
