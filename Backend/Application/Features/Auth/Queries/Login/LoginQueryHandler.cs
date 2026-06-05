using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.Features.Auth.DTOs;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Auth.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginQueryHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var identifier = (request.EmailOrUsername ?? string.Empty).Trim();
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrEmpty(password))
            throw new AuthenticationException("Email/username and password are required.");

        var user = await _userRepository.GetByEmailOrUsernameAsync(identifier);
        if (user is null)
            throw new AuthenticationException("Invalid credentials.");

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            throw new AuthenticationException("Invalid credentials.");

        return AuthResponseDto.FromUser(user);
    }
}
