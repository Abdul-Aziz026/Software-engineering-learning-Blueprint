using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Security;
using Application.Features.Auth.DTOs;
using Application.Settings;
using Contracts.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, MessageResponseDto>
{
    // Same response in every case so the endpoint can't be used to discover which emails are registered.
    private const string GenericResponse =
        "If an account exists for that email, a password reset link has been sent.";

    private readonly IUserRepository _userRepository;
    private readonly IMessageBus _messageBus;
    private readonly PasswordResetOptions _options;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IMessageBus messageBus,
        PasswordResetOptions options,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _messageBus = messageBus;
        _options = options;
        _logger = logger;
    }

    public async Task<MessageResponseDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(email))
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is not null)
            {
                var rawToken = ResetTokenUtil.GenerateRawToken();
                user.RequestPasswordReset(
                    ResetTokenUtil.Hash(rawToken),
                    DateTime.UtcNow.AddMinutes(_options.TokenExpiryMinutes));

                await _userRepository.UpdateAsync(user);

                try
                {
                    // Hand off to the messaging pipeline; a consumer renders + sends off the request thread.
                    var resetLink = $"{_options.FrontendUrl.TrimEnd('/')}/reset-password?token={rawToken}";
                    await _messageBus.PublishAsync(new SendPasswordResetEmail(
                        user.Email.Value,
                        user.Username,
                        resetLink,
                        _options.TokenExpiryMinutes));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to queue password reset email for {UserId}.", user.Id);
                }
            }
        }

        return new MessageResponseDto(GenericResponse);
    }
}
