
namespace Domain.Enums;

public enum UserRole
{
    // Regular signed-in user: can author posts (which start Pending).
    User,
    // Can publish/reject pending posts (which fans a newsletter out to subscribers).
    Admin,
    // Bootstrapped from config. Can assign/revoke the Admin role.
    SuperAdmin
}
