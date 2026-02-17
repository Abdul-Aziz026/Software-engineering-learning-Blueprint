
using Application.Common.Interfaces.Publisher;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : Controller
{

    public HomeController()
    {
    }

    /// <summary>
    /// Gets dashboard overview with key statistics
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        return Ok("Hello Welcome to My Software engineering blueprint");
    }
}
