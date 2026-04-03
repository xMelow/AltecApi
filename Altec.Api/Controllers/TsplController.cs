using Altec.Api.Interface;
using Altec.Api.Records;
using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TsplController : ControllerBase
{
    private readonly ITsplService _tsplService;

    public TsplController(ITsplService tsplService)
    {
        _tsplService = tsplService; 
    }

    [HttpPost("preview")]
    public IActionResult Preview([FromBody] TsplPreviewRequest request)
    {
        byte[] imageBytes = _tsplService.RenderPreview(request.Tspl, request.ShowBlockOutlines, request.Images);
        return File(imageBytes, "image/png");
    }

    [HttpPost("parse")]
    public IActionResult Parse([FromBody] TsplRequest request)
    {
        var commands = _tsplService.Parse(request.Tspl);
        return Ok(new TsplParseResponse(commands));
    }

    [HttpPost("validate")]
    public IActionResult Validate([FromBody] TsplRequest request)
    {
        var result = _tsplService.Validate(request.Tspl);
        return Ok(new TsplValidateResponse(result.IsValid, result.Error));
    }
}