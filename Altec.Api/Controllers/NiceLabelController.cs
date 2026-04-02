using System.Diagnostics;
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
    public async Task<IActionResult> Variables(IFormFile label)
    {
        if (label == null || label.Length == 0) return BadRequest("Label can't be empty");
        try
        {
             var variables = await _niceLabelClient.GetVariables(label);
             return Ok(variables);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting label variables : {ex.Message}" );
        }
    }
    
    [HttpPost("print")]
    public async Task<IActionResult> Print(IFormFile label, [FromForm] int quantity, [FromForm] string? printerName)
    {
        if (label == null || label.Length == 0) return BadRequest("Label file needs to be present");
        if (quantity == null) return BadRequest("Quantity must be present");
        
        try
        {
            await _niceLabelClient.PrintLabel(label, quantity, printerName);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error printing label : {ex.Message}" );
        }
        
        return Ok("Printing label...");
    }
}