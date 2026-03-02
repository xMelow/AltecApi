using Altec.Api.Interface;
using Altec.Api.Records;
using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("tspl")]
public class TsplController : ControllerBase
{
    private readonly ITsplService _tsplService;

    public TsplController(ITsplService tsplService)
    {
        _tsplService = tsplService;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Ok! The TSPL endpoint works!");
    }

    [HttpPost("preview")]
    public IActionResult Preview([FromBody] TsplRequest request)
    {
        byte[] imageBytes = _tsplService.RenderPreview(request.Code);
        return File(imageBytes, "image/png");
    }
}