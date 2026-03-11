using Altec.Api.Domain.Tspl;
using Altec.Api.Interface;
using Altec.Api.Records;

namespace Altec.Api.Services;

public class TsplService : ITsplService
{
    private readonly TsplParser _tsplParser = new TsplParser();
    private readonly TsplRender _tsplRender = new TsplRender();
    private readonly TsplValidator _tsplValidator = new TsplValidator();
    
    public byte[] RenderPreview(string tspl, bool showBlockOuline, Dictionary<string, string> images)
    {
        var tsplCommands = _tsplParser.Parse(tspl);
        return _tsplRender.Render(tsplCommands, showBlockOuline, images);
    }

    public IReadOnlyList<TsplDrawCommand> Parse(string tspl)
    {
        return _tsplParser.Parse(tspl);
    }

    public (bool IsValid, string? Error) Validate(string tspl)
    {
        var result = _tsplValidator.Validate(tspl);
        return (result.IsValid, result.Error);
    }
}