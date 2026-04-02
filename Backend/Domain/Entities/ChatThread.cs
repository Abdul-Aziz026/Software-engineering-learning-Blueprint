
namespace Domain.Entities;

public class ChatThread : BaseEntity
{
    public string ThreadId { get; set; }
    public string Title { get; set; }
    public string CreatedAt { get; set; }
    public string LastMessageAt { get; set; }
}
