using System.Text.RegularExpressions;
using Altec.Api.Records;

namespace Altec.Api.Domain.Tspl;

public class TsplParser
{
    private IReadOnlyList<string> commandsToParse = new[]
    {
        "SIZE", "BAR", "BOX", "TEXT", "BARCODE", "QRCODE", "CIRCLE", "PUTBMP"
    };
    
    public IReadOnlyList<TsplDrawCommand> Parse(string tspl)
    {
        var result = new List<TsplDrawCommand>();
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

    private TsplDrawCommand ParseTsplLine(string line)
    {
        int firstSpace = line.IndexOf(" ");
        var name = line[..firstSpace];
        
        if (firstSpace == -1) return new TsplDrawCommand(name, new List<string>());
        
        return new TsplDrawCommand(name, ParseParameters(line[firstSpace..].Trim()));
    }

    private IReadOnlyList<string> ParseParameters(string arguments)
    {
        var result = new List<string>();
        string currentParam = "";
        bool inQuotes = false;

        foreach (var character in arguments)
        {
            if (character == '\"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            
            if (character == ',' && !inQuotes)
            {
                currentParam = RemoveMetrics(currentParam);
                result.Add(currentParam.Trim());
                currentParam = "";
            }
            else
                currentParam += character;
        }
        if (!string.IsNullOrEmpty(currentParam)) 
            result.Add(RemoveMetrics(currentParam).Trim());
        
        return result;
    }

    private string RemoveMetrics(string parameter)
    {
        return parameter.Replace(" mm", "").Trim();
    }
}