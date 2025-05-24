using System.Text;
using System.Text.Json;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Console = System.Console;

namespace OllamaMcpClient;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("MCP client started");

        McpClientOptions clientOptions = new McpClientOptions()
        {
            ClientInfo = new() { Name = "demo-client", Version = "1.0.0" }
        };

        await using var mcpClient = await McpClientFactory.CreateAsync(
            new StdioClientTransport(
                new StdioClientTransportOptions()
                {
                    Command = "C:\\Users\\Valcor\\h4ck3r\\src\\velcor\\WebMcp\\bin\\Debug\\net9.0\\WebMcp.exe",
                    Name = "webmcp",
                }),
            clientOptions);

        var ollamaChatClient = new OllamaChatClient(
            new Uri("http://localhost:11434"),
            "llama3.2").AsBuilder().UseFunctionInvocation().Build();
        
        IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

        string toolJson = JsonSerializer.Serialize(tools, new JsonSerializerOptions() { WriteIndented = true });

        while (true)
        {
            Console.Write("\n You: ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
                continue;

            if (userInput.Trim().ToLower() == "exit")
            {
                Console.WriteLine("Exiting chat...");
                break;
            }

            var messages = new List<ChatMessage>()
            {
                new(ChatRole.System, "You are a helpful assistant"),
                new(ChatRole.User, userInput)
            };

            try
            {
                ChatResponse response = await ollamaChatClient.GetResponseAsync(
                    messages,
                    new ChatOptions
                    {
                        Tools = [..tools]
                    });

                var assistantMessage = response.Messages.LastOrDefault(m => m.Role == ChatRole.Assistant);

                if (assistantMessage != null)
                {
                    var textOutput = string.Join(" ", assistantMessage.Contents.Select(c => c.ToString()));
                    Console.WriteLine("\n AI: " + textOutput);
                }
                else
                {
                    Console.WriteLine("\n AI: (no assistant message received)");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n Error: " + ex.Message);
            }
        }
    } 
}