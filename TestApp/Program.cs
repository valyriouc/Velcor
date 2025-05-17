using Agent;

namespace TestApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        var content = File.ReadAllText("./secret.txt");
        
        ResearchAgent agent = new ResearchAgent(5, () => new LangSearchCaller(new LangSearchSettings()
        {
            ApiKey = content.Trim(),
            Count = 10,
            Freshness = LangSearchFreshness.OneMonth,
            Summaries = true
        }));
        
        await foreach (var result in agent.ExecuteAsync("How to innovate a company", CancellationToken.None))
        {
            Console.Write(result);
        }
        
        // DuckDuckGoCaller caller = new DuckDuckGoCaller(new DuckDuckGoSettings()
        //     { Format = DuckDuckGoFormat.Json, Pretty = false, NoHtml = true });
        // var response = await caller.PerformAsync("Logic", CancellationToken.None);
        // Console.WriteLine(response);
    }
}