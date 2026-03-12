using Altec.Api.Domain.Printers;
using Altec.Api.Record.Printers;

namespace Altec.Api.Services.Printers;

public class PrinterService : IPrinterService
{
    private readonly PrinterDiscovery _printerDiscovery = new PrinterDiscovery();
    
    public IReadOnlyList<Printer> GetPrinters(List<string> subnets)
    {
        _printerDiscovery.Discover(subnets);
        throw new NotImplementedException();
    }
}