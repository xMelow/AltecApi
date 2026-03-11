using Altec.Api.Domain.Printers;

namespace Altec.Api.Record.Printers;

public record PrinterResponse(IReadOnlyList<Printer> Printers);