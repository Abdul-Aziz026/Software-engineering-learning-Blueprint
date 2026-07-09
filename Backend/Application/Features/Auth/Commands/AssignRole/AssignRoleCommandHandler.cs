using Application.Common.Interfaces.Repositories;
using Application.Features.Auth.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Auth.Commands.AssignRole;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;

    public AssignRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResponseDto> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ActingUserId))
            throw new AuthenticationException("Authentication required.");

        // Authorization: only a SuperAdmin may change roles.
        var actingUser = await _userRepository.GetByIdAsync<User>(request.ActingUserId);
        if (actingUser is null || actingUser.Role != UserRole.SuperAdmin)
            throw new AuthenticationException("Only a super admin can assign roles.");

        var target = await _userRepository.GetByIdAsync<User>(request.TargetUserId)
            ?? throw new NotFoundException("User not found.");

        target.AssignRole(request.Role);
        await _userRepository.UpdateAsync(target);

        return AuthResponseDto.FromUser(target);
    }
}
