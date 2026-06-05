using Application.Common.Interfaces.Repositories;
using Application.Features.Auth.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Auth.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResponseDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ValidationException("User id is required.");

        var user = await _userRepository.GetByIdAsync<User>(request.UserId)
            ?? throw new NotFoundException("User not found.");

        return AuthResponseDto.FromUser(user);
    }
}
