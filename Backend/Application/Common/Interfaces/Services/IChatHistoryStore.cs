
using Microsoft.Extensions.AI;

namespace Application.Common.Interfaces.Services;

public interface IChatHistoryStore
{
    Task<List<ChatMessage>> GetHistoryAsync(string threadId);
    Task SaveChatMessageAsync(string threadId, List<ChatMessage> messages);
    Task<string> CreateThreadAsync();
    Task DeleteThreadAsync(string threadId);
    Task<List<ChatThreadInfo>> GetAllThreadAsync();
}

public class ChatThreadInfo
{
    public string ThreadId { get; set; }
    public string Title { get; set; } = "New chat";
    public DateTime CreatedAt { get; set; } 
    public DateTime LastMessageAt { get; set; }
}
