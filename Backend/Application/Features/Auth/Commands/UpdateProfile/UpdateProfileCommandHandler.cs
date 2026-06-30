using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.Features.Auth.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthValidator _authValidator;

    public UpdateProfileCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthValidator authValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authValidator = authValidator;
    }

    public async Task<AuthResponseDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ValidationException("User id is required.");

        var user = await _userRepository.GetByIdAsync<User>(request.UserId)
            ?? throw new NotFoundException("User not found.");

        var changed = false;
        changed |= await TryApplyUsernameAsync(request.Username, user);
        changed |= await TryApplyEmailAsync(request.Email, user);
        changed |= TryApplyPassword(request.CurrentPassword, request.NewPassword, user);

        if (changed)
        {
            var updated = await _userRepository.UpdateAsync(user);
            if (!updated)
                throw new UnknownException("Failed to update profile.");
        }

        return AuthResponseDto.FromUser(user);
    }

    private async Task<bool> TryApplyUsernameAsync(string? rawUsername, User user)
    {
        if (string.IsNullOrWhiteSpace(rawUsername)) return false;

        var newUsername = rawUsername.Trim().ToLowerInvariant();
        if (newUsername == user.Username) return false;

        var conflict = await _userRepository.GetByUsernameAsync(newUsername);
        if (conflict is not null && conflict.Id != user.Id)
            throw new ValidationException("This username is already taken.");

        user.Rename(newUsername);
        return true;
    }

    private async Task<bool> TryApplyEmailAsync(string? rawEmail, User user)
    {
        if (string.IsNullOrWhiteSpace(rawEmail)) return false;

        var newEmail = rawEmail.Trim().ToLowerInvariant();
        if (!_authValidator.IsValidEmail(newEmail))
            throw new ValidationException("Invalid email address.");

        if (newEmail == user.Email.Value) return false;

        var conflict = await _userRepository.GetByEmailAsync(newEmail);
        if (conflict is not null && conflict.Id != user.Id)
            throw new ValidationException("This email is already in use.");

        user.ChangeEmail(Email.Create(newEmail));
        return true;
    }

    private bool TryApplyPassword(string? currentPassword, string? newPassword, User user)
    {
        if (string.IsNullOrEmpty(newPassword)) return false;

        if (string.IsNullOrEmpty(currentPassword))
            throw new ValidationException("Current password is required to change password.");

        if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt))
            throw new ValidationException("Current password is incorrect.");

        _authValidator.ValidatePassword(newPassword);

        var (hash, salt) = _passwordHasher.HashPassword(newPassword);
        user.SetPassword(hash, salt);
        return true;
    }
}
