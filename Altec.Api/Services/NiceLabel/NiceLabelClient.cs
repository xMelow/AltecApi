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
        using var memoryStream = new MemoryStream();
        await labelFile.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            labelFile.ContentType ?? "application/octet-stream"
        );
        content.Add(fileContent, "labels", labelFile.FileName);

        var request = new HttpRequestMessage(HttpMethod.Post, "nicelabel/variables");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        return result!.AsReadOnly();
    }
}