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

    [HttpPost("variables")]
    public async Task<IActionResult> Variables(IFormFile labelFile)
    {
        if (labelFile == null || labelFile.Length == 0) return BadRequest("Label can't be empty");
        try
        {
             var variables = await _niceLabelClient.GetVariables(labelFile);
             return Ok(variables);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting label variables : {ex.Message}" );
        }
    }
    
    [HttpPost("print")]
    public async Task<IActionResult> Print(IFormFile labelFile, int quantity, string? printerName)
    {
        if (labelFile == null || labelFile.Length == 0) return BadRequest("Label file needs to be present");
        try
        {
            await _niceLabelClient.PrintLabel(labelFile, quantity, printerName);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error printing label : {ex.Message}" );
        }
        
        return Ok("Printing label...");
    }
}