namespace Application.Features.Subscribers.DTOs;

public class SubscriptionResponseDto
{
    public string Message { get; set; } = string.Empty;

    public SubscriptionResponseDto() { }
    public SubscriptionResponseDto(string message) => Message = message;
}
