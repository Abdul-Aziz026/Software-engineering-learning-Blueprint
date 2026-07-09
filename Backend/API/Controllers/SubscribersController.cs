using Application.Common.Interfaces.Publisher;
using Application.Features.Subscribers.Commands.ConfirmSubscription;
using Application.Features.Subscribers.Commands.Subscribe;
using Application.Features.Subscribers.Commands.Unsubscribe;
using Application.Features.Subscribers.DTOs;
using Application.Features.Subscribers.Queries.GetSubscribers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscribersController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public SubscribersController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <summary>Subscribe by email (public). Sends a double opt-in confirmation link.</summary>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionResponseDto>> Subscribe([FromBody] SubscribeRequestDto request)
    {
        var response = await _messageBus.SendAsync<SubscribeCommand, SubscriptionResponseDto>(request.ToSubscribeCommand());
        return Ok(response);
    }

    /// <summary>Confirm a subscription using the token from the confirmation email (public).</summary>
    [AllowAnonymous]
    [HttpGet("confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionResponseDto>> Confirm([FromQuery] string token)
    {
        var response = await _messageBus.SendAsync<ConfirmSubscriptionCommand, SubscriptionResponseDto>(
            new ConfirmSubscriptionCommand { Token = token });
        return Ok(response);
    }

    /// <summary>Unsubscribe using the token from a newsletter email (public).</summary>
    [AllowAnonymous]
    [HttpPost("unsubscribe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionResponseDto>> Unsubscribe([FromQuery] string token)
    {
        var response = await _messageBus.SendAsync<UnsubscribeCommand, SubscriptionResponseDto>(
            new UnsubscribeCommand { Token = token });
        return Ok(response);
    }

    /// <summary>List all subscribers (admin only).</summary>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<SubscriberDto>>> GetAll()
    {
        var subscribers = await _messageBus.SendAsync<GetSubscribersQuery, List<SubscriberDto>>(
            new GetSubscribersQuery { ActingUserId = GetUserId() ?? string.Empty });
        return Ok(subscribers);
    }
}
