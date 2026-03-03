using Altec.Api.Records;

namespace Altec.Api.Interface;

public interface ITsplService
{
    byte[] RenderPreview(string tspl);
    IReadOnlyList<TsplDrawCommand> Parse(string tspl);
}