using Application.Features.Auth.Commands.ForgotPassword;

namespace Application.Features.Auth.DTOs;

public class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;

    public ForgotPasswordCommand ToForgotPasswordCommand() => new() { Email = Email };
}
