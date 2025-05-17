using Agent;
using Agent.Tools;

namespace TestApp;

public class FileWriter : Tooling
{
    public FileWriter(string name, string description) : base(name, description)
    {
        
    }

    public override async Task<string> ExecuteAsync(string input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class Program
{
    static async Task Main(string[] args)
    {
        var content = File.ReadAllText("./secret.txt");
        
        // ResearchAgent agent = new ResearchAgent(5, () => new LangSearchCaller(new LangSearchSettings()
        // {
        //     ApiKey = content.Trim(),
        //     Count = 10,
        //     Freshness = LangSearchFreshness.OneMonth,
        //     Summaries = true
        // }));
        //
        // await foreach (var result in agent.ExecuteAsync("How to innovate a company", CancellationToken.None))
        // {
        //     Console.Write(result);
        // }
        //
        // using var client = new LangSearchCaller(new LangSearchSettings()
        // {
        //     ApiKey = content.Trim(),
        //     Count = 10,
        //     Freshness = LangSearchFreshness.OneMonth,
        //     Summaries = true
        // });
        //
        // var response = await client.PerformAsync("What are llms", CancellationToken.None);

        var dictionary = new Dictionary<string, Tooling>()
        {
            { "FileWriter", new FileWriter("FileWriter", "This writes the content to a file") }
        };
        
        PrototypingAgent agent = new PrototypingAgent(
            "C:\\Users\\Valcor\\h4ck3r\\source\\testing", dictionary);
        
        await foreach (var r in agent.ExecuteAsync("Write a http server in c#", CancellationToken.None))
        {
            Console.WriteLine(r);
        }
        
        // Console.WriteLine(response);
        // DuckDuckGoCaller caller = new DuckDuckGoCaller(new DuckDuckGoSettings()
        //     { Format = DuckDuckGoFormat.Json, Pretty = false, NoHtml = true });
        // var response = await caller.PerformAsync("Logic", CancellationToken.None);
        // Console.WriteLine(response);
    }
}