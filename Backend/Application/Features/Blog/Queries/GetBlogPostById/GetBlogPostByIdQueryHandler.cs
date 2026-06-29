using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Features.Blog.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Blog.Queries.GetBlogPostById;

public class GetBlogPostByIdQueryHandler : IRequestHandler<GetBlogPostByIdQuery, BlogPostDetailDto>
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IBlogCommentRepository _blogCommentRepository;
    private readonly IBlogLikeRepository _blogLikeRepository;
    private readonly ICacheService _cache;

    // Short TTL bounds staleness; write handlers also evict via BlogCacheKeys.Detail.
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    public GetBlogPostByIdQueryHandler(
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

    public async Task<BlogPostDetailDto> Handle(GetBlogPostByIdQuery request, CancellationToken cancellationToken)
    {
        var key = BlogCacheKeys.Detail(request.Id);

        // 1. Try cache. The snapshot never carries a per-user flag.
        var dto = await _cache.GetAsync<BlogPostDetailDto>(key, cancellationToken);

        // 2. Miss -> read from Mongo and populate the cache.
        if (dto is null)
        {
            var post = await _blogPostRepository.GetByIdAsync<BlogPost>(request.Id)
                ?? throw new NotFoundException("Blog post not found.");

            var comments = await _blogCommentRepository.GetByPostIdAsync(post.Id);

            dto = BlogPostDetailDto.FromEntity(post, comments, likedByCurrentUser: false);
            await _cache.SetAsync(key, dto, CacheTtl, cancellationToken);
        }

        // 3. Overlay the per-user like fresh — this part is never cached.
        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            var like = await _blogLikeRepository.GetByPostAndUserAsync(dto.Id, request.UserId);
            dto.LikedByCurrentUser = like is not null;
        }

        return dto;
    }
}
