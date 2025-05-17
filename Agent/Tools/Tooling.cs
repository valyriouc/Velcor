namespace Agent.Tools;

public abstract class Tooling
{
    public string Name { get; }
    
    public string Description { get; }

    public Tooling(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public abstract Task<string> ExecuteAsync(string input, CancellationToken cancellationToken);
}