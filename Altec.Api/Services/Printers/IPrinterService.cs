using Altec.Api.Domain.Printers;

namespace Altec.Api.Services.Printers;

public interface IPrinterService
{
    List<Printer> GetPrinters();
}