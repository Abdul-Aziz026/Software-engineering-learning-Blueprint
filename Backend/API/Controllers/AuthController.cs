using Application.Common.Interfaces.Publisher;
using Application.Features.Auth.Commands.Signup;
using Application.Features.Auth.Commands.UpdateProfile;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Queries.GetUserById;
using Application.Features.Auth.Queries.Login;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public AuthController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>Register a new user.</summary>
    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Signup([FromBody] SignupRequestDto request)
    {
        var response = await _messageBus.SendAsync<SignupCommand, AuthResponseDto>(request.ToSignupCommand());
        return Created(string.Empty, response);
    }

    /// <summary>Authenticate an existing user with email or username.</summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var response = await _messageBus.SendAsync<LoginQuery, AuthResponseDto>(request.ToLoginQuery());
        return Ok(response);
    }

    /// <summary>Get a user's profile by id.</summary>
    [HttpGet("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthResponseDto>> GetProfile(string id)
    {
        var response = await _messageBus.SendAsync<GetUserByIdQuery, AuthResponseDto>(new GetUserByIdQuery { UserId = id });
        return Ok(response);
    }

    /// <summary>Update a user's profile (username, email, password).</summary>
    [HttpPut("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthResponseDto>> UpdateProfile(string id, [FromBody] UpdateProfileRequestDto request)
    {
        var response = await _messageBus.SendAsync<UpdateProfileCommand, AuthResponseDto>(request.ToUpdateProfileCommand(id));
        return Ok(response);
    }
}
