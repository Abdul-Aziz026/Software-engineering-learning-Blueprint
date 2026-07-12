using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Security;
using Contracts.Messaging;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Posts.Commands.PublishPost;

public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMessageBus _messageBus;
    private readonly ICacheService _cache;
    private readonly ILogger<PublishPostCommandHandler> _logger;

    public PublishPostCommandHandler(
        IPostRepository postRepository,
        IUserRepository userRepository,
        IMessageBus messageBus,
        ICacheService cache,
        ILogger<PublishPostCommandHandler> logger)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(PublishPostCommand request, CancellationToken cancellationToken)
    {
        await RoleGuard.EnsureAdminAsync(_userRepository, request.ActingUserId);

        var post = await _postRepository.GetByIdAsync<Post>(request.PostId)
            ?? throw new NotFoundException("Post not found.");

        // Idempotent: republishing an already-published post is a no-op (no duplicate newsletter).
        if (post.Status == PostStatus.Published)
            return;

        post.Status = PostStatus.Published;
        post.PublishedAt = DateTime.UtcNow;
        await _postRepository.UpdateAsync(post);

        // Evict any stale gated snapshot so the now-public post is served fresh.
        await _cache.RemoveAsync(PostCacheKeys.Detail(post.Id), cancellationToken);

        // Fan-out to subscribers happens off the request thread: PostPublishedConsumer loads the
        // confirmed subscribers and emits one newsletter email per recipient. A broker hiccup must
        // not fail publishing (the post is already persisted as Published), so log and move on.
        try
        {
            await _messageBus.PublishAsync(new PostPublished(post.Id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish PostPublished event for post {PostId}.", post.Id);
        }
    }
}
