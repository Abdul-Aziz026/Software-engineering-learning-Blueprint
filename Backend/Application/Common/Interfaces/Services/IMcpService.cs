using Microsoft.Extensions.AI;

namespace Application.Common.Interfaces.Services;

public interface IMcpService
{
    // Tools the connected MCP server exposes (connects on first use).
    Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken ct = default);

    // Executes a named tool with the given arguments, returns its text result (connects on first use).
    Task<string> CallToolAsync(
                        string toolName,
                        Dictionary<string, object?> arguments,
                        CancellationToken ct = default);
}
