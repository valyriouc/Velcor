using System.Text;
using System.Web;

namespace Agent;

public enum DuckDuckGoFormat
{
    Json
}

public sealed class DuckDuckGoSettings
{
    public DuckDuckGoFormat Format { get; init; }

    public bool Pretty { get; init; }

    public bool NoHtml { get; init; }
}

public class DuckDuckGoCaller : WebCaller
{
    private const string BaseUrl = "https://api.duckduckgo.com/";
    
    private DuckDuckGoSettings _settings;
    
    public DuckDuckGoCaller(DuckDuckGoSettings settings) => 
        this._settings = settings;

    protected override HttpRequestMessage BuildRequestMessage(string query)
    {
        var url = BuildUrl(query);
        HttpRequestMessage request = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri(url));
        return request;
    }

    private string BuildUrl(string query)
    {
        StringBuilder sb = new StringBuilder(BaseUrl);
        sb.Append($"?q={HttpUtility.UrlEncode(query)}");
        sb.Append($"&format={_settings.Format.AsString()}");
        sb.Append($"&pretty={(_settings.Pretty ? 1 : 0)}");
        sb.Append($"&no_html={(_settings.NoHtml ? 1 : 0)}");
        sb.Append("&skip_disambig=1");
        return sb.ToString();
    }
}

file static class LocalExtensions
{
    public static string AsString(this DuckDuckGoFormat format)
    {
        return format switch
        {
            DuckDuckGoFormat.Json => "json",
            _ => throw new NotSupportedException()
        };
    }
}