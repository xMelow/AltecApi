using Altec.Api.Record.Printers;
using Altec.Api.Services.Printers;
using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrinterController : ControllerBase
{
    private readonly IPrinterService _printerService;

    public PrinterController(IPrinterService printerService)
    {
        _printerService = printerService; 
    }

    [HttpGet("discover")]
    public async Task<IActionResult> Discover([FromQuery] List<string> subnets)
    {
        var printers = await _printerService.GetPrinters(subnets);
        return Ok(new PrinterResponse(printers));
    }
    
    [HttpGet("{ipAddress}/settings")]
    public async Task<IActionResult> GetPrinterSettings(string ipAddress)
    {
        var info = await _printerService.GetPrinterInfo(ipAddress);
        return Ok(info);
    }

    [HttpPost("{ipAddress}/command")]
    public async Task<IActionResult> SendCommand(string ipAddress, [FromBody] PrinterCommandRequest request)
    {
        var response = await _printerService.SendCommand(ipAddress, request.Command);
        return Ok(new PrinterCommandResponse(response));
    }
}