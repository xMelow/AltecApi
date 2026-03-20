using Altec.Api.Record.Printers;

namespace Altec.Api.Services.Printers;

public interface IPrinterService
{
    Task<IReadOnlyList<Printer>> GetPrinters(List<string> subnets);
}