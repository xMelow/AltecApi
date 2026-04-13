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

        var request = new HttpRequestMessage(HttpMethod.Post, "/nicelabel/variables");
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
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/nicelabel/print");
        request.Content = content;
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task PrintSerialNumbers(IFormFile excelFile, string? printerName)
    {
        var stream = excelFile.OpenReadStream();
        var workbook = new XLWorkbook(stream);
        var sheet1 = workbook.Worksheets.Worksheet("blad1");
        var serialNumbersList = new List<SerialNumberData>();
        var labelFilePath = "./resource/serialNumbersNewPrinters.nsl";

        foreach (var row in sheet1.Rows().Skip(1))
        {
            var sn = row.Cell(1).Value.ToString() ?? "";
            var barcode = row.Cell(2).Value.ToString() ?? "";
            
            serialNumbersList.Add(new SerialNumberData(sn, barcode));
        }

        serialNumbersList = serialNumbersList.OrderBy(serialData => int.Parse(new string(serialData.SerialNumber.Where(char.IsDigit).ToArray()))).ToList();
        
        var fileStream = File.OpenRead(_config["LabelPaths:SerialNewPrintersLabel"]);

        foreach (var serialNumberData in serialNumbersList)
        {
            var requestData = new MultipartFormDataContent();
            var variables = new Dictionary<string, string>
            {
                ["sn"] = serialNumberData.SerialNumber,
                ["barcode"] = serialNumberData.Barcode
            };

            var variablesJson = JsonSerializer.Serialize(variables);
            requestData.Add(new StringContent(variablesJson), "variables");
            
            fileStream.Position = 0;
            StreamContent labelStream = new StreamContent(fileStream);
            requestData.Add(labelStream, "label");
            
            if (printerName != null)
                requestData.Add(new StringContent(printerName), "printerName");
            
            var request = new HttpRequestMessage(HttpMethod.Post, "/nicelabel/printLabelVariables");
            request.Content = requestData;
        
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            request.Content.Dispose();
        }
    }
}