
using Application.Features.Auth.Commands.Signup;

namespace Application.Features.Auth.DTOs;

public class SignupRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public SignupCommand ToSignupCommand()
    {
        return new SignupCommand
        {
            Username = Username,
            Email = Email,
            Password = Password
        };
    }
}
