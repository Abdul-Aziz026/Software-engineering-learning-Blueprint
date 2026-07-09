using Application.Features.Subscribers.Commands.Subscribe;

namespace Application.Features.Subscribers.DTOs;

public class SubscribeRequestDto
{
    public string Email { get; set; } = string.Empty;

    public SubscribeCommand ToSubscribeCommand() => new() { Email = Email };
}
