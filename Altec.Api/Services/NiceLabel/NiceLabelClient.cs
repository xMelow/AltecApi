using System.Net.Http.Headers;

namespace Altec.Api.Services.NiceLabel;

public class NiceLabelClient : INiceLabelClient
{
    private readonly HttpClient _httpClient;

    public NiceLabelClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<string>> GetVariables(IFormFile labelFile)
    {
        using var stream = labelFile.OpenReadStream();

        var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(labelFile.ContentType ?? "application/octet-stream");

        content.Add(fileContent, "labels", labelFile.FileName);

        var response = await _httpClient.PostAsync(
            "http://localhost:44368/nicelabel/variables",
            content
        );

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        return result!.AsReadOnly();
    }
}