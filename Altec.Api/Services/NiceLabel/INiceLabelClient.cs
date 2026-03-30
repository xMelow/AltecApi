namespace Altec.Api.Services.NiceLabel;

public interface INiceLabelClient
{ 
    Task<IReadOnlyList<string>> GetVariables(IFormFile labelFile);
    Task PrintLabel(IFormFile labelFile, string? printerIpAddress);
}