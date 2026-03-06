using Altec.Api.Records;

namespace Altec.Api.Interface;

public interface ITsplService
{
    byte[] RenderPreview(string tspl, bool showBlockOutline);
    IReadOnlyList<TsplDrawCommand> Parse(string tspl);
}