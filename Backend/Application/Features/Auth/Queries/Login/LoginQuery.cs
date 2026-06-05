
using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Queries.Login;

public class LoginQuery : IRequest<AuthResponseDto>
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
