using Altec.Api.Record.Printers;

namespace Altec.Api.Services.Printers;

public interface IPrinterService
{
    Task<IReadOnlyList<Printer>> GetPrinters(List<string> subnets);
    Task<PrinterInfo> GetPrinterInfo(string ipAddress);
    Task<string> SendCommand(string ipAddress, string command);
}