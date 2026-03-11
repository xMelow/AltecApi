namespace Altec.Api.Records;

public record TsplPreviewRequest(
    string Tspl, 
    Dictionary<string, string>? Images = null,
    bool ShowBlockOutlines = false
);
