using System.Net.Http.Headers;
using System.Text.Json;
using Altec.Api.Record.NiceLabel;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Altec.Api.Services.NiceLabel;

public class NiceLabelClient : INiceLabelClient
{
    private readonly HttpClient _httpClient;

    public NiceLabelClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
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
}