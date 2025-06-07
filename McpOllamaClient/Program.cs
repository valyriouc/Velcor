namespace McpOllamaClient;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        McpServerFactory.Instance.Register(
            "C:\\Users\\Valcor\\h4ck3r\\src\\velcor\\WebMcp\\bin\\Debug\\net9.0\\WebMcp.exe",
            new McpServerConfig() { Name = "webmcp", TransportType = McpTransportType.Stdio});

        app.UseHttpsRedirection();
        app.MapDefaultControllerRoute();
        app.Run();
    }
}