using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("tspl")]
public class TsplController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Ok! This is the tpsl endpoint");
    }

    [HttpPost("preview")]
    public IActionResult Preview([FromBody] TsplRequest request)
    {
        byte[] imageBytes = [];
        return File(imageBytes, "image/png");
    }
}

public record TsplRequest(string Code);