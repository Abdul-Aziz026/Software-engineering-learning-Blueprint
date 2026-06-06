using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Security;
using Application.Features.Auth.DTOs;
using Application.Settings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, MessageResponseDto>
{
    // Same response in every case so the endpoint can't be used to discover which emails are registered.
    private const string GenericResponse =
        "If an account exists for that email, a password reset link has been sent.";

    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly PasswordResetOptions _options;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IEmailSender emailSender,
        PasswordResetOptions options,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
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
                user.PasswordResetTokenHash = ResetTokenUtil.Hash(rawToken);
                user.PasswordResetTokenExpiresAt =
                    DateTime.UtcNow.AddMinutes(_options.TokenExpiryMinutes);

                await _userRepository.UpdateAsync(user);

                try
                {
                    var resetLink = $"{_options.FrontendUrl.TrimEnd('/')}/reset-password?token={rawToken}";
                    await _emailSender.SendAsync(
                        user.Email,
                        "Reset your password",
                        BuildEmailBody(user.Username, resetLink, _options.TokenExpiryMinutes),
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    // Don't surface email-delivery failures to the caller (would leak account existence
                    // and expose internals); log for diagnostics instead.
                    _logger.LogError(ex, "Failed to send password reset email to {UserId}.", user.Id);
                }
            }
        }

        return new MessageResponseDto(GenericResponse);
    }

    private static string BuildEmailBody(string username, string resetLink, int expiryMinutes) => $@"
        <div style=""font-family:Arial,sans-serif;font-size:15px;color:#1a1a1a;line-height:1.6"">
            <p>Hi {username},</p>
            <p>We received a request to reset your password. Click the button below to choose a new one:</p>
            <p style=""margin:24px 0"">
                <a href=""{resetLink}""
                   style=""background:#2563eb;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;display:inline-block"">
                    Reset password
                </a>
            </p>
            <p style=""font-size:13px;color:#666"">
                This link expires in {expiryMinutes} minutes. If you didn't request a reset, you can safely ignore this email.
            </p>
            <p style=""font-size:13px;color:#666"">If the button doesn't work, paste this link into your browser:<br>{resetLink}</p>
        </div>";
}
