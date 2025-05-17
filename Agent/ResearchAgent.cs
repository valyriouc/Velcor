using System.Runtime.CompilerServices;
using System.Text.Json;
using OllamaSharp.Models.Chat;

namespace Agent;

file class SearchQueryObject
{
    public string Query { get; set; }
    
    public string Rationale { get; set; }
}

public class ResearchAgent(int researchCicles, Func<WebCaller> buildWebCaller)
{
    public async IAsyncEnumerable<string> ExecuteAsync(string researchTask, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using Ollama client = new Ollama(OllamaDefaults.DefaultEndpoint);
        using WebCaller web = buildWebCaller.Invoke();
        
        List<Message> history = [ 
            new(ChatRole.System, "You are an research assistent which is responsible to find as many information as possible about a given topic and generate a summary highlighting the most important information"),
            new(ChatRole.User, Prompts.GenerateQueryWriterPrompt(DateTime.UtcNow, researchTask))
        ];
        
        int internalCounter = 0;
        while (internalCounter < researchCicles)
        {
            ChatRequest request = new ChatRequest()
            {
                Format = "json",
                Messages = history,
                Model = "llama3.2:latest",
                Stream = false,
            };

            List<Message> responses = await client.ChatAsync(request, cancellationToken).ToListAsync();
            if (responses.Count == 1)
            {
                var message = responses[0];
                history.Add(message);
                var webQuery = JsonSerializer.Deserialize<SearchQueryObject>(message.Content ?? throw new Exception("Content must be provided"), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (webQuery is null)
                {
                    continue;
                }

                Console.WriteLine($"Follow up web query: {webQuery.Query}");
                string searchResult = await web.PerformAsync(webQuery.Query, cancellationToken);
                string toolOutput =
                    $"""
                    The web research returned the following results:
                    {searchResult} 
                    """;
                history.Add(new Message(ChatRole.Tool, toolOutput));
                history.Add(new Message(ChatRole.User, Prompts.GenerateReflectionPrompt(researchTask)));
            }
            
            internalCounter++;
        }
        
        history.Add(
            new Message(ChatRole.User, Prompts.SummarizerPrompt));
        
        ChatRequest summaryRequest = new()
        {
            Format = "json",
            Messages = history,
            Model = "llama3.2:latest",
            Stream = true,
        };

        List<Message> end = await client.ChatAsync(summaryRequest, cancellationToken).ToListAsync();
        foreach (var message in end)
        {
            if (message.Content is null)
            {
                continue;
            }
            
            yield return message.Content;
        }
    }
}

internal static class AsyncEnumerableExtensions
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