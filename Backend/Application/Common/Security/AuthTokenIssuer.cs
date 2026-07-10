using Application.Common.Interfaces.Security;
using Application.Features.Auth.DTOs;
using Application.Settings;
using Domain.Entities;

namespace Application.Common.Security;

/// <inheritdoc cref="IAuthTokenIssuer" />
public sealed class AuthTokenIssuer : IAuthTokenIssuer
{
    private readonly IJwtTokenGenerator _generator;
    private readonly JwtOptions _options;

    public AuthTokenIssuer(IJwtTokenGenerator generator, JwtOptions options)
    {
        _generator = generator;
        _options = options;
    }

    public AuthResponseDto Issue(User user)
    {
        var access = _generator.GenerateAccessToken(user);

        var rawRefresh = ResetTokenUtil.GenerateRawToken();
        user.SetRefreshToken(ResetTokenUtil.Hash(rawRefresh), DateTime.UtcNow.AddDays(_options.RefreshTokenDays));

        return AuthResponseDto.FromUser(user).WithTokens(access.Token, rawRefresh, access.ExpiresAtUtc);
    }
}
