using Agent;
using Agent.Tools;

namespace TestApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        var content = await File.ReadAllTextAsync("./secret.txt");
        
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
            { "FileWriter", new FileWriter() }
        };
        
        PrototypingAgent agent = new PrototypingAgent(
            "C:\\Users\\Valcor\\h4ck3r\\src\\testing", dictionary);
        
        await foreach (var r in agent.ExecuteAsync(
                           "Write a fully functional http parser in .NET 8.0 using c#", CancellationToken.None))
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