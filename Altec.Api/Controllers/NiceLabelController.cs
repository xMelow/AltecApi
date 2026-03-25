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

    [HttpPost("print")]
    public async Task<IActionResult> Print(IFormFile file)
    {
        _niceLabelService.PrintLabel(file);
        return Ok();
    }
}