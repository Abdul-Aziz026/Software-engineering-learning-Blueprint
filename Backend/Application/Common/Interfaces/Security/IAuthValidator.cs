namespace Application.Common.Interfaces.Security;

public interface IAuthValidator
{
    bool IsValidEmail(string email);
    void ValidatePassword(string password);
}
