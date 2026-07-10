using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.Common.Security;
using Application.Features.Auth.DTOs;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthTokenIssuer _tokenIssuer;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IAuthTokenIssuer tokenIssuer)
    {
        _userRepository = userRepository;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new AuthenticationException("Refresh token is required.");

        var tokenHash = ResetTokenUtil.Hash(request.RefreshToken);
        var user = await _userRepository.GetByRefreshTokenHashAsync(tokenHash);

        // Reject unknown/expired tokens the same way — no distinction leaked.
        if (user is null || !user.IsRefreshTokenValid(tokenHash, DateTime.UtcNow))
            throw new AuthenticationException("Invalid or expired refresh token.");

        // Rotate: Issue stamps a new refresh-token hash on the user, invalidating the one just used.
        var response = _tokenIssuer.Issue(user);
        await _userRepository.UpdateAsync(user);

        return response;
    }
}
