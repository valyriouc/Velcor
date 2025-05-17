using System.Runtime.CompilerServices;
using System.Text;
using Agent.Tools;
using OllamaSharp.Models.Chat;
using ChatRole = OllamaSharp.Models.Chat.ChatRole;

namespace Agent;

public class PrototypingAgent : IAgent, IDisposable
{
    // information 
    // reasoning 
    // planning 
    // csharp prototype 
    private const string model = "deepseek-r1:8b";
    
    private readonly string _workingDirectory;
    private readonly Dictionary<string, Tooling> _tools;
    
    private readonly Ollama _client = new(OllamaDefaults.DefaultEndpoint, 1000);
    
    public PrototypingAgent(string workingDirectory, Dictionary<string, Tooling> tools)
    {
        if (!Directory.Exists(workingDirectory))
        {
            throw new DirectoryNotFoundException();
        }
        
        this._workingDirectory = workingDirectory;
        this._tools = tools;
    }
    
    public async IAsyncEnumerable<string> ExecuteAsync(string query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        List<Message> history = new();
        
        history.Add(new Message(ChatRole.System, BuildSystemPrompt
            (query, 
                _tools.Values.Select(x => $"{x.Name} - {x.Description}").ToList())));

        var request = new ChatRequest()
        {
            Messages = history,
            Model = model,
            Stream = true,
        };

        await foreach (var message in _client.ChatAsync(request, cancellationToken))
        {
            Console.Write(message.Content);
        }
        
        yield break;
    }

    private static string BuildSystemPrompt(string query, List<string> tools)
    {
        StringBuilder sb = new();
        foreach (var tool in tools)
        {
            sb.AppendLine(tool);
        }
        
        return $$"""
               Your goal is to generate a complete and fully functional csharp prototype for the provided idea 
               
               <TOPIC>
               {{query}}
               </TOPIC>
               
               <PROCEDURE>
               1. Make a detailed plan how you want to achieve the goal
               2. If more information is needed then search for them 
               3. Use the tools you have a available to create the prototype
               </PROCEDURE>
               
               <TOOLS>
               You have a lot of tools available to complete your task. 
               Here you find a list of the tools with a description:
               {{sb}}
               </TOOLS>
               
               <EXAMPLE>
               Example output for a tool use:
               {
                   "tool": "FileWriter",
                   "parameter": {
                        "filepath": "test.txt",
                        "content": "Hello World"
                   }
               }
               </EXAMPLE>
               
               Provide your response in JSON format:
               """;
    }

    public void Dispose() => _client.Dispose();
}

