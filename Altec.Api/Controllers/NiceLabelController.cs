using Altec.Api.Services.NiceLabel;
using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NiceLabelController : ControllerBase
{

    private readonly INiceLabelService _niceLabelService;

    public NiceLabelController(INiceLabelService niceLabelService)
    {
        _niceLabelService = niceLabelService;
    }

    [HttpPost("print/label")]
    public async Task<IActionResult> Discover([FromBody] string labelFile)
    {
        _niceLabelService.PrintLabel(labelFile);
        return Ok();
    }
}