using System.Text;

namespace Agent.Tools;

public class DotnetTooling : Tooling
{
    public DotnetTooling(string name, string description) : base(name, description)
    {
        
    }
    
    public override string CreateToolSchema()
    {
        string schema =
            $$"""
            {
                "name": "{{this.Name}}",
                "description": "{{this.Description}}", 
                "version": "1.0.0",
                "parameter": {
                    "type": "object",
                    "properties": {
                        
                    }
                }
            }
            """;

        return schema;
    }

    public override ToolParameter CreateParameter(string input)
    {
        throw new NotImplementedException();
    }

    public override Task<string> ExecuteAsync(ToolParameter input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}