using System.Text;
using System.Text.Json;

namespace Agent.Tools;

public class FileWriter : Tooling
{
    public FileWriter() 
        : base(
            "FileWriter", 
            "Overrides content of existing file or creates a new one and stores the specified content in it")
    {
        
    }
    
    public override string CreateToolSchema()
    {
        string schema =
            $$"""
              {
                  "name": "{{Name}}",
                  "description": "{{Description}}",
                  "parameters": {
                      "type": "object",
                      "properties": {
                        "filePath": "The path to the file where the content should be stored",
                        "content": "The content to store in the specified file"
                      }
                  }
              }
              """;

        return schema;
    }

    public override ToolParameter CreateParameter(string input)
    {
        FileWriterParameter? obj = JsonSerializer.Deserialize<FileWriterParameter>(input);
        if (obj is null)
        {
            throw new InvalidOperationException("Could not create parameter obj for FileWriter!");
        }

        return obj;
    }
    
    public override async Task<string> ExecuteAsync(ToolParameter parameter, CancellationToken cancellationToken)
    {
        try
        {
            if (parameter is not FileWriterParameter param)
            {
                return $"""
                       Tool parameter schema was incorrect! See the {this.Name} docs for more information:
                       {this.CreateToolSchema()}
                       """;
            }

            param.Validate();

            await using var stream = File.Open(param.FilePath, FileMode.Create);

            await stream.WriteAsync(Encoding.UTF8.GetBytes(param.Content), cancellationToken);
            await stream.FlushAsync(cancellationToken);
            
            return $"""
                    Tool execution was successful!
                    Content was stored in file {param.FilePath}
                    """;
        }
        catch (Exception ex)
        {
            return $"""
                    Error message: {ex.Message}
                    Please try to fix this issue! Here the tool documentation:
                    {this.CreateToolSchema()}
                    """;
        }
    }
}

public sealed class FileWriterParameter : ToolParameter
{
    public string FilePath { get; set; }

    public string Content { get; set; }
    
    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            throw new ArgumentException("FilePath is required and can't be empty");
        }

        if (string.IsNullOrWhiteSpace(Content))
        {
            throw new ArgumentException("Content is required and can't be empty");
        }
    }
}