
using System.Runtime.CompilerServices;
using OllamaSharp;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;

namespace Agent;

public sealed class Ollama : IDisposable
{
    private readonly OllamaApiClient _client;
    private readonly HttpClient _httpClient = new();

    public Ollama(string url, uint timeout)
    {
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
        _httpClient.BaseAddress = new Uri(url);
        _client = new OllamaApiClient(_httpClient);
    }


    /// <summary>
    /// Sends the prompt to ollama using the default model to generate a response
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<string> GenerateAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var request = new GenerateRequest
        {
            Prompt = prompt,
            Model = OllamaDefaults.DefaultModel
        };

        await foreach (var response in this.GenerateAsync(request, cancellationToken))
        {
            yield return response;
        }
    }
    
    /// <summary>
    /// Sends the specified generation request to ollama and returns the response 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<string> GenerateAsync(GenerateRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach(GenerateResponseStream? response in this._client.GenerateAsync(request, cancellationToken: cancellationToken))
        {
            if (response is null)
            {
                continue;
            }
            
            yield return response!.Response;
        }
    }

    /// <summary>
    /// Chats with the models using the chat request 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<Message> ChatAsync(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var response in this._client.ChatAsync(request, cancellationToken: cancellationToken))
        {
            if (response is null)
            {
                continue;
            }
            
            yield return response.Message;
        }
    }
    
    public void Dispose()
    {
        _client.Dispose();
        _httpClient.Dispose();
    }
}

public static class OllamaDefaults
{
    public const string DefaultEndpoint = "http://localhost:11434";

    public const string DefaultModel = "llama3.2:latest";

}