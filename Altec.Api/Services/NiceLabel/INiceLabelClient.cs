namespace Altec.Api.Services.NiceLabel;

public interface INiceLabelClient
{ 
    Task<IReadOnlyList<string>> GetVariables(IFormFile labelFile);
    Task PrintLabel(IFormFile labelFile,int quantity, string? printerIpAddress);
    Task PrintSerialNumbers(IFormFile excelFile, string? printerName);
}