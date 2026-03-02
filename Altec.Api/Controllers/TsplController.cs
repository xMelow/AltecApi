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
}

public record TsplRequest(string Code);