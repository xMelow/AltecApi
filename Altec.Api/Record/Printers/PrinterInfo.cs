namespace Altec.Api.Record.Printers;

public record PrinterInfo(
    int Dpi,
    string Model,
    string Serial,
    string Version,
    string Mileage,
    int LabelCounter,
    string MacAddressNet,
    string IpAddressNet,
    string NetworkName,
    string SensorType,
    int Speed,
    int Density,
    string LabelWidth,
    string LabelHeight,
    string GapSize,
    string GapSizeOffset,
    int BlineSize,
    string Direction,
    string Ribbon,
    int Offset,
    int ShiftX,
    int ShiftY,
    string CountryCode,
    string CodePage,
    int GapOffset
);
