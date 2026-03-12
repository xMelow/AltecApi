using Altec.Api.Record.Printers;

namespace Altec.Api.Services.Printers;

public interface IPrinterService
{
    IReadOnlyList<Printer> GetPrinters();
}