using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ModelContextProtocol.Client;

namespace McpOllamaClient;

public class McpServerFactory
{
    private static McpServerFactory? _instance;
    
    public static McpServerFactory Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = new McpServerFactory();
            }

            return _instance;
        }
    }
    
    private readonly Dictionary<string, McpServerConfig> _mcpServers = new();

    private McpServerFactory()
    {
        
    }

    /// <summary>
    /// Registers a new mcp server for the chatbot
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Register(string command, McpServerConfig name)
    {
        if (!_mcpServers.TryAdd(command, name))
        {
            throw new InvalidOperationException($"Command {command} is already available!");
        }
    }
    
    /// <summary>
    /// Get all mcp tools from all registered mcp servers 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<McpClientTool> GetToolsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach ((string command, McpServerConfig config) in _mcpServers)
        {
            IMcpClient client = await McpClientFactory.CreateAsync(
                config.CreateTransport(command),
                new McpClientOptions() { ClientInfo = new() { Name = "demo-client", Version = "1.0.0" } },
                cancellationToken: cancellationToken);

            IList<McpClientTool> tools = await client.ListToolsAsync(cancellationToken: cancellationToken);
            foreach (var tool in tools)
            {
                yield return tool;
            }
        }
    }

    /// <summary>
    /// Lists all mcp servers currently registered in the mcp factory 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<McpServer> GetServers()
    {
        foreach (var (command, config) in _mcpServers)
        {
            yield return new McpServer(command, config);
        }
    }
    
    /// <summary>
    /// Stores the current configuration of mcp servers to the disk
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        // todo: 
    }

    /// <summary>
    /// Load the current configuration of mcp servers from the disk
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        // todo: 
    }
}

public class McpServer
{
    public string Command { get; }
    
    public McpServerConfig Config { get; }

    [JsonConstructor]
    public McpServer(string command, McpServerConfig config)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            throw new ArgumentNullException(nameof(command));
        }
        
        Command = command;
        Config = config ?? throw new ArgumentNullException(nameof(config));
    }
}

public enum McpTransportType
{
    Stdio,
    Web
}
    
public class McpServerConfig
{
    public string Name { get; set; }
    
    public McpTransportType TransportType { get; set; }
    
}

internal static class Extensions
{
    public static IClientTransport CreateTransport(this McpServerConfig config, string command)
    {
        return config.TransportType switch
        {
            McpTransportType.Stdio => new StdioClientTransport(
                new StdioClientTransportOptions() { Command = command, Name = config.Name }),
            McpTransportType.Web => throw new NotSupportedException("Web tools are currently not available"),
            _ => throw new NotImplementedException()
        };
    }
}