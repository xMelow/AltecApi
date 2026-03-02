using Altec.Api.Records;

namespace Altec.Api.Domain.Tspl;

public class TsplParser
{
    private IReadOnlyList<string> commandsToParse = new[]
    {
        "SIZE", "BAR", "BOX", "TEXT", "BARCODE", "QRCODE", "CIRCLE", "PUTBMP"
    };
    
    public IReadOnlyList<TsplCommand> Parse(string tspl)
    {
        var result = new List<TsplCommand>();
        string[] lines = tspl.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var name = line.Split(" ")[0];
            if (commandsToParse.Contains(name))
            {
                result.Add(ParseTsplLine(line));
            }
        }
        return result;
    }

    private TsplCommand ParseTsplLine(string line)
    {
        var name = line.Split(" ")[0];
        string[] data = line.Split(" ")[1].Split(",");

        return new TsplCommand(name, data);
    }
}