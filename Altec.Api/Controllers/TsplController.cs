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
    public IActionResult Preview([FromBody] TsplPreviewRequest request)
    {
        byte[] imageBytes = _tsplService.RenderPreview(request.Tspl, request.ShowBlockOutlines);
        return File(imageBytes, "image/png");
    }

    [HttpPost("parse")]
    public IActionResult Parse([FromBody] TsplRequest request)
    {
        var commands = _tsplService.Parse(request.Tspl);
        return Ok(new TsplParseResponse(commands));
    }
}