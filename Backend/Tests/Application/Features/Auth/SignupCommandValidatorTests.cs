using Application.Features.Auth.Commands.Signup;
using FluentValidation.TestHelper;

namespace Tests.Application.Features.Auth;

// Tests the SignupCommandValidator in isolation — no MediatR, no MongoDB, no HTTP.
// FluentValidation.TestHelper gives a fluent API over a single TestValidate() run.
public class SignupCommandValidatorTests
{
    private readonly SignupCommandValidator _validator = new();

    private static SignupCommand ValidCommand() => new()
    {
        Username = "aziz",
        Email = "aziz@example.com",
        Password = "supersecret"
    };

    [Fact]
    public void Valid_command_has_no_errors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing@dotcom")]
    public void Invalid_email_is_rejected(string email)
    {
        var command = ValidCommand();
        command.Email = email;

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Too_short_username_is_rejected(string username)
    {
        var command = ValidCommand();
        command.Username = username;

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Password_shorter_than_4_chars_is_rejected()
    {
        var command = ValidCommand();
        command.Password = "sho";

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
