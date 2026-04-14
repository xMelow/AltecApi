using System.Net.Http.Headers;
using System.Text.Json;
using Altec.Api.Record.NiceLabel;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Altec.Api.Services.NiceLabel;

public class NiceLabelClient : INiceLabelClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public NiceLabelClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<IReadOnlyList<string>> GetVariables(IFormFile labelFile)
    {
        var fileStream = labelFile.OpenReadStream();
        StreamContent streamContent = new StreamContent(fileStream);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/nicelabel/variables");
        request.Content = streamContent;
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        return result!.AsReadOnly();
    }

    public async Task PrintLabel(IFormFile labelFile, int quantity, string? printerName)
    {
        var fileStream = labelFile.OpenReadStream();
        fileStream.Position = 0;
        StreamContent streamContent = new StreamContent(fileStream);

        var content = new MultipartFormDataContent();
        content.Add(streamContent, "label");
        content.Add(new StringContent(quantity.ToString()), "quantity");
        if (printerName != null)
            content.Add(new StringContent(printerName), "printerName");
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/nicelabel/print");
        request.Content = content;
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public Task GetLabelPreview(IFormFile labelFile)
    {
        throw new NotImplementedException();
    }

    public async Task PrintSerialNumbers(IFormFile excelFile, string printerType, string? printerName)
    {
        var excelData = ReadExcelData(excelFile, printerType);
        var requestData = BuildPrintSerialRequest(excelData, printerName);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/nicelabel/printLabelVariables");
        request.Content = requestData;
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private List<SerialNumberData> ReadExcelData(IFormFile excelFile, string printerType)
    {
        var stream = excelFile.OpenReadStream();
        var workbook = new XLWorkbook(stream);
        var sheet1 = workbook.Worksheets.Worksheet("blad1");
        var serialNumbersList = new List<SerialNumberData>();

        foreach (var row in sheet1.Rows().Skip(1))
        {
            var sn = row.Cell(1).Value.ToString() ?? "";
            var mac = row.Cell(2).Value.ToString() ?? "";
            var type = printerType;
            
            serialNumbersList.Add(new SerialNumberData(sn, mac, type));
        }
        
        return serialNumbersList.OrderBy(serialData => int.Parse(new string(serialData.SerialNumber.Where(char.IsDigit).ToArray()))).ToList();
    }

    private MultipartFormDataContent BuildPrintSerialRequest(List<SerialNumberData> serialNumbersList, string? printerName)
    {
        var requestData = new MultipartFormDataContent();
        var fileStream = File.OpenRead(_config["LabelPaths:SerialNewPrintersLabel"]);
        
        var allVariables = serialNumbersList.Select(s => new Dictionary<string, string> {
            ["sn"] = s.SerialNumber,
            ["mac"] = s.MacAddress,
            ["type"] = s.Type
        }).ToList();
        
        var json = JsonSerializer.Serialize(allVariables);
        requestData.Add(new StringContent(json), "variables");
        
        StreamContent labelStream = new StreamContent(fileStream);
        requestData.Add(labelStream, "label");
            
        if (printerName != null)
            requestData.Add(new StringContent(printerName), "printerName");
        
        return requestData;
    }
}