using Microsoft.AspNetCore.Mvc;

namespace McpOllamaClient.Controllers;

[ApiController]
[Route("api/[controller]")]
public class McpController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => 
        Ok(McpServerFactory.Instance.GetServers());

    [HttpPost]
    public IActionResult Post([FromBody] McpServer mcpServer)
    {
        McpServerFactory.Instance.Register(mcpServer.Command, mcpServer.Config);
        return Ok(mcpServer);
    }
}