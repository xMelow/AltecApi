using System.Diagnostics;
using Altec.Api.Record.NiceLabel;
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
            return Ok("Printing label...");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error printing label : {ex.Message}" );
        }
    }

    [HttpPost("labelPreview")]
    public async Task<IActionResult> LabelPreview(IFormFile label)
    {
        try
        {
            await _niceLabelClient.GetLabelPreview(label);
            return Ok("Label preview");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating label preview : {ex.Message}" );
        }
    }

    [HttpPost("automations/serialNumbersNewPrinters")]
    public async Task<IActionResult> SerialNumbersNewPrinters(IFormFile excelFile, [FromForm] string printerType, [FromForm] string? printerName)
    {
        if (excelFile == null || excelFile.Length == 0) return BadRequest("Excel file must be present");
        if (printerType == null) return BadRequest("Printer Type must be present");

        try
        {
            await _niceLabelClient.PrintSerialNumbers(excelFile, printerType, printerName);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }
}