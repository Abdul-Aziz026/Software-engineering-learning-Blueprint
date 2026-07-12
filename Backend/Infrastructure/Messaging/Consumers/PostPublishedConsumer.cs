using Application.Common.Interfaces.Repositories;
using Application.Settings;
using Contracts.Messaging;
using Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers;

/// <summary>
/// Fan-out stage. Turns one <see cref="PostPublished"/> event into one <see cref="SendNewsletterEmail"/>
/// per confirmed subscriber, so every recipient's delivery is retried / dead-lettered independently.
/// Producing messages (not sending) means a crash mid-fan-out just redelivers the event and re-queues
/// the per-recipient messages — no partial batch, no giant single message.
/// </summary>
public class PostPublishedConsumer : IConsumer<PostPublished>
{
    private readonly IPostRepository _postRepository;
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly PasswordResetOptions _options; // reused for its FrontendUrl (frontend base URL)
    private readonly ILogger<PostPublishedConsumer> _logger;

    public PostPublishedConsumer(
        IPostRepository postRepository,
        ISubscriberRepository subscriberRepository,
        PasswordResetOptions options,
        ILogger<PostPublishedConsumer> logger)
    {
        _postRepository = postRepository;
        _subscriberRepository = subscriberRepository;
        _options = options;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PostPublished> context)
    {
        var post = await _postRepository.GetByIdAsync<Post>(context.Message.PostId);
        if (post is null)
        {
            _logger.LogWarning("PostPublished received for missing post {PostId}; skipping fan-out.",
                context.Message.PostId);
            return;
        }

        var subscribers = await _subscriberRepository.GetConfirmedAsync();
        if (subscribers.Count == 0)
            return;

        var frontend = _options.FrontendUrl.TrimEnd('/');
        var postLink = $"{frontend}/posts/{post.Id}";

        foreach (var subscriber in subscribers)
        {
            var unsubscribeLink = $"{frontend}/unsubscribe?token={subscriber.UnsubscribeToken}";
            await context.Publish(new SendNewsletterEmail(
                subscriber.Email.Value,
                post.Title,
                post.Summary,
                postLink,
                unsubscribeLink));
        }

        _logger.LogInformation("Newsletter fan-out queued for post {PostId} to {Count} subscribers.",
            post.Id, subscribers.Count);
    }
}
