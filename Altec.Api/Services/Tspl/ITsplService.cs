using Altec.Api.Records;

namespace Altec.Api.Interface;

public interface ITsplService
{
    byte[] RenderPreview(string tspl, bool showBlockOutline, Dictionary<string, string> images);
    IReadOnlyList<TsplDrawCommand> Parse(string tspl);
    (bool IsValid, string? Error) Validate(string tspl);
}