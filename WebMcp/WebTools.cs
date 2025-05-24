using System.ComponentModel;
using ModelContextProtocol.Server;

namespace WebMcp;

[McpServerToolType]
public class WebTools
{
    [McpServerTool, Description("Executes a GET request on the given url")]
    public async Task<string> GetAsync(
        [Description("The url which should be retrieved via GET")]
        string url)
    {
        try
        {
            Uri uri = new(url);
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"""
                    An error occured: {ex.Message}
                    """;
        }
    }

    [McpServerTool, Description("Executes a POST request on the given url")]
    public async Task<string> PostAsync(
        [Description("The url which should be retrieved via POST")]
        string url,
        [Description("The content of the request")]
        string content)
    {
        try
        {
            Uri uri = new(url);
            using HttpClient client = new();
            HttpResponseMessage response = await client.PostAsync(uri, new StringContent(content));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"""
                    An error occured: {ex.Message}
                    """;
        }
    }
}
