using Altec.Api.Interface;
using Altec.Api.Record.Printers;
using Altec.Api.Services.Printers;
using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("printers")]
public class PrinterController : ControllerBase
{
    private readonly IPrinterService _printerService;

    public PrinterController(IPrinterService printerService)
    {
        _printerService = printerService; 
    }

    [HttpGet]
    public IActionResult Get()
    {
        var printers = _printerService.GetPrinters();
        return Ok(new PrinterResponse(printers));
    }
}