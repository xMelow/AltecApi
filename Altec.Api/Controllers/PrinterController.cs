using Altec.Api.Interface;
using Altec.Api.Services.Printer;
using Microsoft.AspNetCore.Mvc;

namespace Altec.Api.Controllers;

[ApiController]
[Route("printer")]
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
        return Ok("Ok! The printer endpoint works!");
    }
}