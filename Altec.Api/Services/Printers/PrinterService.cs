using System.Net;
using Altec.Api.Domain.Printers;
using Altec.Api.Record.Printers;

namespace Altec.Api.Services.Printers;

public class PrinterService : IPrinterService
{
    private readonly PrinterDiscovery _printerDiscovery;
    
    public PrinterService(PrinterDiscovery printerDiscovery)
    {
        _printerDiscovery = printerDiscovery;
    }
    
    public async Task<IReadOnlyList<Printer>> GetPrinters(List<string> subnets)
    {
        var result = await _printerDiscovery.Discover(subnets);
        return result;
    }

    public async Task<PrinterInfo> GetPrinterInfo(string ipAddress)
    {
        var ip = IPAddress.Parse(ipAddress);
        return await _printerDiscovery.GetPrinterSettings(ip);
    }

    public async Task<string> SendCommand(string ipAddress, string command)
    {
        var ip = IPAddress.Parse(ipAddress);
        return await _printerDiscovery.SendPrinterCommand(ip, command);
    }
}