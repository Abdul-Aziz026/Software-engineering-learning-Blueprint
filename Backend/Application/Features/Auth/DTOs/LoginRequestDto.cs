
using Application.Features.Auth.Queries.Login;

namespace Application.Features.Auth.DTOs;

public class LoginRequestDto
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public LoginQuery ToLoginQuery()
    {
        return new LoginQuery
        {
            EmailOrUsername = EmailOrUsername,
            Password = Password
        };
    }
}
