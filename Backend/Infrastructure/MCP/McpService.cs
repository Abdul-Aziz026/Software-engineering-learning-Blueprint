using Application.Common.Interfaces.Services;
using Infrastructure.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Infrastructure.MCP;

public class McpService : IMcpService, IAsyncDisposable
{
    private readonly SemaphoreSlim _connectGate = new(1, 1);

    private McpClient? _client;
    private IClientTransport? _transport;
    private IReadOnlyList<AITool>? _toolsCache;

    private readonly McpServerOptions _options;
    private readonly ILogger<McpService> _logger;

    public McpService(IOptions<McpServerOptions> options, ILogger<McpService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken ct = default)
    {
        await EnsureConnectedAsync(ct);
        return _toolsCache!;
    }

    public async Task<string> CallToolAsync(string toolName, Dictionary<string, object?> arguments, CancellationToken ct = default)
    {
        await EnsureConnectedAsync(ct);

        _logger.LogInformation("Calling MCP tool. Name={ToolName}.", toolName);
        var result = await _client!.CallToolAsync(toolName, arguments, cancellationToken: ct);

        return string.Join("\n",
            result.Content
            .OfType<TextContentBlock>()
            .Select(c => c.Text));
    }

    /// <summary>
    /// Connects the in-process MCP client on first use and caches its tool list.
    /// Safe to call concurrently — the connect happens at most once.
    /// </summary>
    private async Task EnsureConnectedAsync(CancellationToken ct)
    {
        if (_client is not null)
            return;

        await _connectGate.WaitAsync(ct);
        try
        {
            if (_client is not null)
                return;

            if (string.IsNullOrWhiteSpace(_options.Endpoint))
            {
                throw new InvalidOperationException(
                    "MCP is not configured: set McpServer:Endpoint to the Streamable HTTP URL (e.g. http://localhost:5000/mcp).");
            }

            var endpoint = new Uri(_options.Endpoint, UriKind.Absolute);
            _transport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Name = "MCP Streamable HTTP",
                Endpoint = endpoint,
                TransportMode = HttpTransportMode.StreamableHttp,
            });

            _client = await McpClient.CreateAsync(_transport, clientOptions: null, loggerFactory: null, cancellationToken: ct);
            _toolsCache = (await _client.ListToolsAsync(cancellationToken: ct)).Cast<AITool>().ToList();
            _logger.LogInformation("MCP client connected. Endpoint={Endpoint}, toolCount={Count}.", endpoint, _toolsCache.Count);
        }
        finally
        {
            _connectGate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _toolsCache = null;

        if (_client is not null)
        {
            await _client.DisposeAsync();
            _client = null;
        }

        if (_transport is IAsyncDisposable asyncTransport)
        {
            await asyncTransport.DisposeAsync();
        }
        _transport = null;

        _connectGate.Dispose();
    }
}
