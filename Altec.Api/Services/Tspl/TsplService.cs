using Altec.Api.Domain.Tspl;
using Altec.Api.Interface;
using Altec.Api.Records;

namespace Altec.Api.Services;

public class TsplService : ITsplService
{
    private readonly TsplParser _tsplParser;
    private readonly TsplRender _tsplRender;
    private readonly TsplValidator _tsplValidator;
    
    public TsplService(TsplParser tsplParser, TsplRender tsplRender, TsplValidator tsplValidator)
    {
        _tsplParser = tsplParser;
        _tsplRender = tsplRender;
        _tsplValidator = tsplValidator;
    }
    
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