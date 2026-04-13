namespace Altec.Api.Record.NiceLabel;

public record PrintSerialNumbersResponse(
    int TotalPrinted, 
    List<string> SerialNumbers, 
    List<string> Failures
);