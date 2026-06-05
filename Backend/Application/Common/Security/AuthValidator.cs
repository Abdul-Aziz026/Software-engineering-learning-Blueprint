using System.Text.RegularExpressions;
using Application.Common.Interfaces.Security;
using Domain.Exceptions;

namespace Application.Common.Security;

public class AuthValidator : IAuthValidator
{
    private static readonly Regex EmailRegex = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        RegexOptions.Compiled);

    public bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }

    public void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 3)
            throw new ValidationException("Password must be at least 3 characters long.");

        bool hasLetter = false;
        bool hasDigit = false;
        bool hasSpecial = false;

        foreach (var c in password)
        {
            if (char.IsLetter(c)) hasLetter = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)) hasSpecial = true;
        }

        if (!hasLetter)
            throw new ValidationException("Password must contain at least one letter.");
        if (!hasDigit)
            throw new ValidationException("Password must contain at least one number.");
        if (!hasSpecial)
            throw new ValidationException("Password must contain at least one special character.");
    }
}
