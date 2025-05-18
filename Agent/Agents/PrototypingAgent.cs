using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Agent.Tools;
using OllamaSharp.Models.Chat;
using ChatRole = OllamaSharp.Models.Chat.ChatRole;

namespace Agent;

public static class PrototypingAgencyPrompts
{
    public static string GeneratePlanningPrompt(
        string application)
    {
        string planning =
            $"""
            You are a highly skilled software and planning architect. Your task is it to create a detailed plan 
            to develop the architecture for a application {application}.
            
            <GOAL>
            1. Identify everything that is needed to create the application.
            2. Create proposals for interfaces between the different components of the application.
            3. Output a detailed implementation plan with all technical details. This plan will later be given to a software developer
            who is responsible for the actual implementation.
            </GOAL> 
            
            <REQUIREMENTS>
            Ensure that you consider every detail of the system.
            </REQUIREMENTS>
            
            <FORMAT>
            Format your response as a markdown spec that contains every step, class, etc.
            so the developer can easily follow this plan to create the application.
            </FORMAT>
            
            <TASK>
            Reflect critically on your decisions so the best possible implementation plan is created.
            </TASK>
            """;

        return planning;
    }

    public static string ReflectPlanPrompt(string application)
    {
        string reflect =
            $"""
            You are a critical thinker that is responsible for improving the given implementation 
            plan for the application {application}.
            
            <GOAL>
            - Identify gaps in the plan that could break the implementation of the application.
            - Question every assumption in the plan to find out if the plan considers the best approaches, technologies, etc.
            </GOAL>
            
            <FORMAT>
            Output a list which all your identified gaps and questions
            </FORMAT>
            """;

        return reflect;
    } 
    
    public static string GenerateProgrammingPrompt(
        string language,
        string application,
        Dictionary<string, Tooling> tools)
    {
        string coding =
            $"""
            You are a high-performance programmer skilled in the programming language {language}.
            Your goal is it to program the application {application}. For this purpose you follow 
            the plan that is given to you.
            
            <TOPIC>
            {application}
            </TOPIC> 
            
            <REQUIREMENTS>
            Ensure that your application is completely functional and follows the best practices of software development.
            Do not create empty classes or methods that need further development. You need to implement everything needed.
            </REQUIREMENTS>
            
            <FORMAT>
            
            </FORMAT>
            """;

        return coding;
    }

    public static string GenerateTestingPrompt()
    {
        string testing =
            """
            
            """;

        return testing;
    }
}

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
        
        // history.Add(new Message(ChatRole.System, BuildSystemPrompt
        //     (query, 
        //         _workingDirectory,
        //         _tools.Values.Select(x => $"{x.Name} - {x.Description}").ToList())));
        history.Add(
            new Message(ChatRole.System,
                PrototypingAgencyPrompts.GeneratePlanningPrompt(query)));
        
        var request = new ChatRequest()
        {
            Messages = history,
            Model = model,
            Stream = true,
        };

        StringBuilder sb = new();

        bool now = false;
        await foreach (var message in _client.ChatAsync(request, cancellationToken))
        {
            if (message.Content is null)
            {
                continue;
            }

            if (message.Content.Contains("</think>"))
            {
                now = true;
                continue;
            }

            if (now)
            {
                sb.Append(message.Content);
            }
        }

        string plan = sb.ToString().Trim();

        history =
        [
            new Message(ChatRole.System, PrototypingAgencyPrompts.ReflectPlanPrompt(query))
            new         
        ];
        
        
        
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