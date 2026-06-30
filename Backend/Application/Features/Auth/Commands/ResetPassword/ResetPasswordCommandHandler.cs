using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.Common.Security;
using Application.Features.Auth.DTOs;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, MessageResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthValidator _authValidator;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthValidator authValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authValidator = authValidator;
    }

    public async Task<MessageResponseDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var token = (request.Token ?? string.Empty).Trim();
        var newPassword = request.NewPassword ?? string.Empty;

        if (string.IsNullOrWhiteSpace(token))
            throw new ValidationException("Reset token is required.");

        // Validate the password before touching the DB so weak passwords fail fast.
        _authValidator.ValidatePassword(newPassword);

        var tokenHash = ResetTokenUtil.Hash(token);
        var user = await _userRepository.GetByPasswordResetTokenHashAsync(tokenHash);

        if (user is null
            || user.PasswordResetTokenExpiresAt is null
            || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
        {
            throw new ValidationException("This password reset link is invalid or has expired.");
        }

        var (hash, salt) = _passwordHasher.HashPassword(newPassword);

        // Sets the new credentials AND clears the token in one step — the entity
        // itself guarantees the reset link is single-use (can't be replayed).
        user.CompletePasswordReset(hash, salt);

        var updated = await _userRepository.UpdateAsync(user);
        if (!updated)
            throw new UnknownException("Failed to reset password.");

        return new MessageResponseDto("Your password has been reset. You can now sign in with your new password.");
    }
}
