using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.Features.Auth.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Auth.Commands.Signup;

public class SignupCommandHandler : IRequestHandler<SignupCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthValidator _authValidator;

    public SignupCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthValidator authValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authValidator = authValidator;
    }

    public async Task<AuthResponseDto> Handle(SignupCommand request, CancellationToken cancellationToken)
    {
        var username = (request.Username ?? string.Empty).Trim();
        var email = (request.Email ?? string.Empty).Trim();
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username))
            throw new ValidationException("Username is required.");

        if (!_authValidator.IsValidEmail(email))
            throw new ValidationException("Invalid email address.");

        _authValidator.ValidatePassword(password);

        var normalizedEmail = email.ToLowerInvariant();
        var normalizedUsername = username.ToLowerInvariant();

        if (await _userRepository.GetByEmailAsync(normalizedEmail) is not null)
            throw new ValidationException("A user with this email is already registered.");

        if (await _userRepository.GetByUsernameAsync(normalizedUsername) is not null)
            throw new ValidationException("A user with this username is already registered.");

        var (hash, salt) = _passwordHasher.HashPassword(password);

        var user = new User
        {
            Username = normalizedUsername,
            Email = normalizedEmail,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        var added = await _userRepository.AddAsync(user);
        if (!added)
            throw new UnknownException("Failed to register user.");

        return AuthResponseDto.FromUser(user);
    }
}
