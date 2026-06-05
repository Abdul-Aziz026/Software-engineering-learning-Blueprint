
using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<AuthResponseDto>
{
    public string UserId { get; set; } = string.Empty;
}
