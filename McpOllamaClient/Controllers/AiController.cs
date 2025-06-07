using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace McpOllamaClient.Controllers;

public class ChatRequest
{
    public string Model { get; set; } 
    
    public string Prompt { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    [HttpPost("chat/")]
    public async Task ChatAsync(
        [FromBody] ChatRequest request)
    {
        using IChatClient ollama = new OllamaChatClient(
            new Uri("http://localhost:11434/"),
            request.Model)
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        List<McpClientTool> tools = await McpServerFactory.Instance.GetToolsAsync(CancellationToken.None).ToListAsync();

        await foreach (ChatResponseUpdate message in ollama.GetStreamingResponseAsync([new ChatMessage(ChatRole.User, request.Prompt)],
                           new ChatOptions() { Tools = [..tools] }))
        {
            await Response.WriteAsync(message.Text);
            await Response.Body.FlushAsync();
        }
    }
    
    [HttpPost("generate/")]
    public async Task GenerateAsync(
        [FromBody] ChatRequest request)
    {
        // todo: 
    }
}

internal static class Extensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        List<T> result = [];
        await foreach (var item in source)
        {
            result.Add(item);
        }
        return result;
    }
}