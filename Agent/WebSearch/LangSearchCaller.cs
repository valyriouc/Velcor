using System.Net.Http.Headers;

namespace Agent;

public enum LangSearchFreshness
{
    OneDay,
    OneWeek,
    OneMonth,
    OneYear,
    NoLimit
}

public sealed class LangSearchSettings
{
    public string ApiKey { get; init; }
    
    public LangSearchFreshness Freshness { get; init; } = LangSearchFreshness.NoLimit;
    
    public bool Summaries { get; init; }

    public uint Count { get; init; } = 10;
}

public class LangSearchCaller : WebCaller
{
    private const string Endpoint = "https://api.langsearch.com/v1/web-search";

    private readonly LangSearchSettings _settings;
    
    public LangSearchCaller(LangSearchSettings settings)
    {
        _settings = settings;
    }
    
    protected override HttpRequestMessage BuildRequestMessage(string query)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            throw new Exception("LangSearch requires an api key!");
        }
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        string body =
            $$"""
              {
                  "query": "{{query}}",
                  "freshness": "{{_settings.Freshness.AsString()}}",
                  "summary": {{_settings.Summaries.ToString().ToLower()}},
                  "count": {{_settings.Count}}
              }
              """;
        request.Content = new StringContent(body);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return request;
    }
}

file static class LocalExtensions
{
    public static string AsString(this LangSearchFreshness self)
    {
        return self switch
        {
            LangSearchFreshness.OneDay => "oneDay",
            LangSearchFreshness.OneWeek => "oneWeek",
            LangSearchFreshness.OneMonth => "oneMonth",
            LangSearchFreshness.OneYear => "oneYear",
            LangSearchFreshness.NoLimit => "noLimit",
            _ => throw new NotImplementedException()
        };
    }
}