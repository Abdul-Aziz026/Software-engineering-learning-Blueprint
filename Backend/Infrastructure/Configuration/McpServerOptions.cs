namespace Infrastructure.Configuration;

/// <summary>Client settings for connecting to this app's MCP HTTP endpoint (Streamable HTTP).</summary>
public class McpServerOptions
{
    public string Endpoint { get; set; }
}
