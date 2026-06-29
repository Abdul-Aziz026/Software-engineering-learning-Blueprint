using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Blog.Commands.DeleteBlogPost;

public class DeleteBlogPostCommandHandler : IRequestHandler<DeleteBlogPostCommand>
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IBlogCommentRepository _blogCommentRepository;
    private readonly IBlogLikeRepository _blogLikeRepository;
    private readonly ICacheService _cache;

    public DeleteBlogPostCommandHandler(
        IBlogPostRepository blogPostRepository,
        IBlogCommentRepository blogCommentRepository,
        IBlogLikeRepository blogLikeRepository,
        ICacheService cache)
    {
        _blogPostRepository = blogPostRepository;
        _blogCommentRepository = blogCommentRepository;
        _blogLikeRepository = blogLikeRepository;
        _cache = cache;
    }

    public async Task Handle(DeleteBlogPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _blogPostRepository.GetByIdAsync<BlogPost>(request.Id)
            ?? throw new NotFoundException("Blog post not found.");

        if (post.AuthorId != request.UserId)
            throw new ValidationException("You can only delete your own posts.");

        // Cascade: remove the post's comments and likes so we don't leave orphans.
        var comments = await _blogCommentRepository.GetByPostIdAsync(post.Id);
        foreach (var comment in comments)
            await _blogCommentRepository.DeleteByIdAsync<BlogComment>(comment.Id);

        var likes = await _blogLikeRepository.GetByPostIdAsync(post.Id);
        foreach (var like in likes)
            await _blogLikeRepository.DeleteByIdAsync<BlogLike>(like.Id);

        await _blogPostRepository.DeleteByIdAsync<BlogPost>(post.Id);

        // The post is gone — drop its cached snapshot so a miss returns NotFound, not stale data.
        await _cache.RemoveAsync(BlogCacheKeys.Detail(post.Id), cancellationToken);
    }
}
