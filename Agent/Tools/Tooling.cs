namespace Agent.Tools;

public abstract class Tooling(string name, string description)
{
    public string Name { get; } = name;

    public string Description { get; } = description;

    public abstract string CreateToolSchema();

    public abstract ToolParameter CreateParameter(string input);
    
    public abstract Task<string> ExecuteAsync(ToolParameter input, CancellationToken cancellationToken);
}

public abstract class ToolParameter
{
    /// <summary>
    /// Validates the parameter object retrieved from the llm and throws exceptions if something is not correct
    /// </summary>
    public abstract void Validate();
    
}