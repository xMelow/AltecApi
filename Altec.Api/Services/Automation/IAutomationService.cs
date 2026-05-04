namespace Altec.Api.Services.Automation;

public interface IAutomationService
{
    Task PrintSerialNumbers(IFormFile excelFile, string printerType, string? printerName);
}