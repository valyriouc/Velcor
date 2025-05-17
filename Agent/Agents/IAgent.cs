namespace Agent;

public interface IAgent
{
    public IAsyncEnumerable<string> ExecuteAsync(string query, CancellationToken cancellationToken);
}