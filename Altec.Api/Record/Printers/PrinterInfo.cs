namespace Altec.Api.Record.Printers;

public record PrinterInfo(int Speed, int Density, int PaperWidth, int PaperHeight, string MediaSensor, int Gap, int GapOffset, string PostPrintAction, string Direction, int Offset, int ShiftX, int ShiftY, string Ribbon, string RibbonSensor, string CodePage, string CountryCode);