
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Chat.DTOs;

public class ChatResponseDto
{
    public string ThreadId { get; set; }
    public string Answer { get; set; }
    public LlmProvider Provider { get; set; }
    public IReadOnlyList<ToolCallRecord> ToolCalls { get; set; }
}
