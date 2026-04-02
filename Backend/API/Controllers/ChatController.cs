using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Services;
using Application.Features.Chat.Commands;
using Application.Features.Chat.DTOs;
using Application.Tools.DTOs;
using Application.Tools.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IMessageBus _messageBus;
    private readonly IChatHistoryStore _historyStore;

    public ChatController(IMessageBus messageBus
        , IChatHistoryStore chatHistoryStore)
    {
        _messageBus = messageBus;
        _historyStore = chatHistoryStore;
    }

    // ── POST /api/chat ───────────────────────────────────────────────────────
    // Request:  { "query": "What is the weather in London?", "provider": "Gemini" }
    // Response: { "answer": "...", "provider": "Gemini", "toolCalls": [...] }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
    {
        var command = new SendChatCommand
        {
            ThreadId = request.ThreadId,
            Query = request.Query,
            Provider = request.Provider
        };
        var result = await _messageBus.SendAsync<SendChatCommand, ChatResponseDto>(command);
        return Ok(result);
    }

    // ── GET /api/chat/threads — list all threads ─────────────────────
    [HttpGet("threads")]
    public async Task<IActionResult> GetThreads()
    {
        var threads = await _historyStore.GetAllThreadAsync();
        return Ok(threads);
    }

    // ── DELETE /api/chat/threads/{threadId} — delete a thread ────────
    [HttpDelete("threads/{threadId}")]
    public async Task<IActionResult> DeleteThread(string threadId)
    {
        await _historyStore.DeleteThreadAsync(threadId);
        return NoContent();
    }

    // ── GET /api/chat/tools ──────────────────────────────────────────────────
    // Returns the list of tools the MCP server currently exposes.
    // Useful for debugging — see what tools Gemini/Claude can use.
    [HttpGet("tools")]
    public async Task<IActionResult> GetTools()
    {
        var query = new GetAvailableToolsQuery();
        var tools = await _messageBus.SendAsync<GetAvailableToolsQuery, List<ToolSummary>>(query);
        return Ok(tools);
    }
}