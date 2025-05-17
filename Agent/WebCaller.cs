namespace Agent;


/// <summary>
/// This class provides the base for interacting with the web
/// </summary>
public abstract class WebCaller : IDisposable
{
    private readonly HttpClient _httpClient = new();

    public async Task<string> PerformAsync(string query, CancellationToken cancellationToken)
    {
        using var request = BuildRequestMessage(query);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return string.Empty;
        }
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    protected abstract HttpRequestMessage BuildRequestMessage(string query);
    
    public void Dispose() => _httpClient.Dispose();
}