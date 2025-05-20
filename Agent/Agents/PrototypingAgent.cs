using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Agent.Prompting;
using Agent.Tools;
using OllamaSharp.Models;
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
        history.Add(
            new Message(ChatRole.System,
                PrototypingAgencyPrompts.GeneratePlanningPrompt(query)));
        
        ChatRequest request = new ChatRequest()
        {
            Messages = history,
            Model = model,
            Stream = true,
        };

        RemarksResponse remarksObj;

        // planning round 
        int counter = 0;
        string path;
        do
        {
            string plan = await _client
                .ChatAsync(request, cancellationToken)
                .CutThinkingAsync()
                .ToLlmResponseAsync();

            counter++;

            path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                $"plan{counter}.txt");
            
            await File.WriteAllTextAsync(
                path,
                plan,
                cancellationToken);
            
            string prompt =
                $"""
                 Here is the plan you need to reflect on:
                 {plan}
                 """;

            GenerateRequest validatePlan = new GenerateRequest()
            {
                System = PrototypingAgencyPrompts.ReflectPlanPrompt(query),
                Prompt = prompt,
                Model = "llama3.2",
                Format = "json",
                Stream = true,
            };
        
            string remarks = await _client
                .GenerateAsync(validatePlan, cancellationToken)
                .ToLlmResponseAsync();
            
            Console.WriteLine(remarks);
            
            remarksObj = JsonSerializer.Deserialize<RemarksResponse>(remarks, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) 
                         ?? throw new Exception("Not correct remarks response");

            StringBuilder sb = new();
            foreach (var remark in remarksObj.Remarks)
            {
                sb.AppendLine($"* {remark}");
            }

            string m =
                $"""
                 For this plan i have the following remarks:
                 {sb}
                 Please adapt the plan according to my notes
                 """;
            
            history.Add(new Message(
                ChatRole.User, 
                m));
            
        } while (!remarksObj.IsGood && counter < 1);
        
        string endPlan = await File.ReadAllTextAsync(path, cancellationToken);

        GenerateRequest codingRequest = new GenerateRequest()
        {
            System = PrototypingAgencyPrompts.GenerateProgrammingPrompt(query),
            Prompt = $"Please implement the plan in {endPlan}",
            Model = "qwen2.5-coder:14b"
        };

        await foreach (var coding in _client.GenerateAsync(codingRequest, cancellationToken))
        {
            Console.Write(coding);
        }
        
        yield break;
    }

    private static string BuildSystemPrompt(
        string query, 
        string rootPath,
        List<string> tools)
    {
        StringBuilder sb = new();
        foreach (string tool in tools)
        {
            sb.AppendLine(tool);
        }
        
        // a planning agent 
        
        // a coding agent 
        
        // a tool usage agent 
        
        
        return $$"""
               Your goal is to generate a complete and fully functional csharp prototype for the provided idea.
               You create all resources within the directory {{rootPath}}. 
               
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
               Please surround every tool use with <tool></tool>
               </TOOLS>
               
               <EXAMPLE>
               Example output for a tool use:
               <tool name="FileWriter">
                <parameter>
                    <filepath>test.txt</filepath>
                    <content>
                        Hello world
                    </content>
                </parameter>
               </tool>
               
               </EXAMPLE>
               
               Please provide your output in xml format 
               """;
    }

    public void Dispose() => _client.Dispose();
}

public class ToolResponse
{
    public string Tool { get; set; }
    
    public string Parameter { get; set; }
}

public class RemarksResponse
{
    public bool IsGood { get; set; }
    
    public List<string> Remarks { get; set; }
}

file static class LocalExtensions
{
    public static async IAsyncEnumerable<Message> CutThinkingAsync(this IAsyncEnumerable<Message> messages)
    {
        bool isThinking = true;
        await foreach (var message in messages)
        {
            if (message.Content is null)
            {
                continue;
            }

            if (message.Content.ToLower().Contains("</think>"))
            {
                isThinking = false;
                continue;
            }

            if (!isThinking)
            {
                yield return message;
            }
        } 
    }
    
    public static async Task<string> ToLlmResponseAsync(this IAsyncEnumerable<Message> messages)
    {
        StringBuilder sb = new();
        
        await foreach (var message in messages)
        {
            if (message.Content is null)
            {
                continue;
            }    
            
            sb.Append(message.Content);
        }
        
        return sb.ToString().Trim();
    }

    public static async Task<string> ToLlmResponseAsync(this IAsyncEnumerable<string> messages)
    {
        StringBuilder sb = new();

        await foreach (var message in messages)
        {
            sb.Append(message);
        }
        
        return sb.ToString().Trim();
    }
}