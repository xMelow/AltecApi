using Altec.Api.Services.Automation;
using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutomationController : ControllerBase
{
    private readonly IAutomationService _automationService;

    public AutomationController(IAutomationService automationService)
    {
        _automationService = automationService;
    }

    [HttpPost("serialNumbersNewPrinters")]
    public async Task<IActionResult> SerialNumbersNewPrinters(IFormFile excelFile, [FromForm] string printerType, [FromForm] string? printerName)
    {
        if (excelFile == null || excelFile.Length == 0) return BadRequest("Excel file must be present");
        if (printerType == null) return BadRequest("Printer Type must be present");

        try
        {
            await _automationService.PrintSerialNumbers(excelFile, printerType, printerName);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }
}
