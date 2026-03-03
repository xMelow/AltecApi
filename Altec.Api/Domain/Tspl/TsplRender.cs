using Altec.Api.Records;

namespace Altec.Api.Domain.Tspl;

public class TsplRender
{
    private const int Dpi = 300;
    
    public byte[] Render(IReadOnlyList<TsplDrawCommand> commands)
    {
        var sizeCommand = commands.FirstOrDefault(command => command.Name == "SIZE")
            ?? throw new InvalidOperationException("SIZE command is required");
        
        var width = Mm2Pixels(int.Parse(sizeCommand.Arguments[0]));
        var height = Mm2Pixels(int.Parse(sizeCommand.Arguments[1]));
        
        throw new NotImplementedException();
    }

    private int Mm2Pixels(int dots)
    {
        return Convert.ToInt32(Math.Round((dots / 25.4) * Dpi));
    }
}