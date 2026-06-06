using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<MessageResponseDto>
{
    public string Email { get; set; } = string.Empty;
}
