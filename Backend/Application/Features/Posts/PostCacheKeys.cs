namespace Application.Features.Posts;

/// <summary>
/// Single source of truth for post-related cache keys. Both the read path
/// (GetPostByIdQueryHandler, which populates the cache) and every write
/// path (Update / Delete / ToggleLike / AddComment / DeleteComment, which must
/// evict it) format the key here — so the producer and the invalidators can
/// never drift apart and leave a stale entry behind.
/// </summary>
public static class PostCacheKeys
{
    /// <summary>Key for a single post's cached detail snapshot (user-independent).</summary>
    public static string Detail(string postId) => $"post:detail:{postId}";

    /// <summary>Key for a post's cached AI summary (longer, ~24h TTL — see GetPostAiSummary).</summary>
    public static string AiSummary(string postId) => $"post:aisummary:{postId}";
}
