using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    // The authenticated user's id, from the JWT 'sub' claim (mapped to NameIdentifier).
    protected string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
}
