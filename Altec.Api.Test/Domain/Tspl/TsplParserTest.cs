using Altec.Api.Domain.Tspl;
using Altec.Api.Records;
using Xunit.Abstractions;

namespace Altec.Api.Test.Domain.Tspl;

public class TsplParserTest
{
    private readonly TsplParser _parser = new();
    private readonly ITestOutputHelper _output;

    public TsplParserTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private void AssertCommand(TsplDrawCommand expected, TsplDrawCommand actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Arguments, expected.Arguments);
    }

    [Fact]
    public void Parse_TextCommand_ReturnsCorrectCommand()
    {
        var tspl = "TEXT 10,10,\"3\",0,1,1,\"Hello Altec\"";
        var expected = new TsplDrawCommand("TEXT", new List<string> { "10", "10", "3", "0", "1", "1", "Hello Altec" });
        
        var result = _parser.Parse(tspl);

        AssertCommand(expected, result[0]);
    }

    [Fact]
    public void Parse_FullLabel_ReturnsCorrectCommands()
    {
        var tspl =
            "SIZE 103 mm,110 mm\nGAP 3 mm,0 mm\nREFERENCE 0,0\nSPEED 2.0\nDENSITY 8\nSET RIBBON ON\nSET PEEL OFF\nSET CUTTER OFF\nSET PARTIAL_CUTTER OFF\nSET TEAR ON\nSET REWIND OFF\nDIRECTION 0,0\nSHIFT 0,0\nOFFSET 0 mm\n\nCLS\nTEXT 427, 47,\"0\",0, 16,16,\"Hello, World\"\nBAR 33,135,1199,7\nBOX 33,178, 1232,890,6\nQRCODE 938,951,L,14,A,0,M2,S7,\"123456789012\"\nCIRCLE 933,590,260,12\nTEXT 77,234,\"0\",0,16,16,\"Name:\"\nTEXT 77,322,\"0\",0,16,16,\"Phone number:\"";
        var expected = new List<TsplDrawCommand>
        {
            new TsplDrawCommand("SIZE", new List<string> { "103 mm", "110 mm" }),
            new TsplDrawCommand("TEXT", new List<string> { "427", "47", "0", "0", "16", "16", "Hello, World" }),
            new TsplDrawCommand("BAR", new List<string> { "33", "135", "1199", "7" }),
            new TsplDrawCommand("BOX", new List<string> { "33", "178", "1232", "890", "6" }),
            new TsplDrawCommand("QRCODE", new List<string> { "938", "951", "L", "14", "A", "0", "M2", "S7", "123456789012" }),
            new TsplDrawCommand("CIRCLE", new List<string> { "933", "590", "260", "12" }),
            new TsplDrawCommand("TEXT", new List<string> { "77", "234", "0", "0", "16", "16", "Name:" }),
            new TsplDrawCommand("TEXT", new List<string> { "77", "322", "0", "0", "16", "16", "Phone number:" }),
        };

        var result = _parser.Parse(tspl);
        
        Assert.Equal(expected.Count, result.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            AssertCommand(expected[i], result[i]);
        }
    }
}