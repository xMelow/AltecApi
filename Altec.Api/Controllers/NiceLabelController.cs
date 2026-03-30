using Altec.Api.Services.NiceLabel;
using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NiceLabelController : ControllerBase
{
    private readonly INiceLabelClient _niceLabelClient;

    public NiceLabelController(INiceLabelClient niceLabelClient)
    {
        _niceLabelClient = niceLabelClient;
    }

    [HttpPost("print")]
    public async Task<IActionResult> Print(IFormFile labelFile)
    {
        var response = await _niceLabelClient.PrintLabel(labelFile);
        return Ok();
    }

    [HttpPost("variables")]
    public async Task<IActionResult> Variables(IFormFile label)
    {
        if (label == null || label.Length == 0) return BadRequest("Body can't be empty");
        
        var variables = await _niceLabelClient.GetVariables(label);
        return Ok(variables);
    }
}