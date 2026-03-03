using Altec.Api.Domain.Tspl;
using Altec.Api.Interface;
using Altec.Api.Records;

namespace Altec.Api.Services;

public class TsplService : ITsplService
{
    private readonly TsplParser _tsplParser = new TsplParser();
    private readonly TsplRender _tsplRender = new TsplRender();
    
    public byte[] RenderPreview(string tspl)
    {
        var tsplCommands = _tsplParser.Parse(tspl);
        // var labelPreview = _tsplRender.Render(tsplCommands);
        
        throw new NotImplementedException();
    }

    public IReadOnlyList<TsplDrawCommand> Parse(string tspl)
    {
        return _tsplParser.Parse(tspl);
    }
}