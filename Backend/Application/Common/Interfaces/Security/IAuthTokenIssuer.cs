using Application.Features.Auth.DTOs;
using Domain.Entities;

namespace Application.Common.Interfaces.Security;

/// <summary>
/// Mints an access token and rotates the user's single refresh token, stamping the new
/// refresh-token hash onto <paramref name="user"/>. The caller is responsible for persisting
/// the user afterwards (AddAsync on signup, UpdateAsync on login/refresh).
/// </summary>
public interface IAuthTokenIssuer
{
    AuthResponseDto Issue(User user);
}
