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

    [Fact]
    public void Parse_TextCommand_ReturnsCorrectCommand()
    {
        var tspl = "TEXT 10,10,\"3\",0,1,1,\"Hello Altec\"";
        var expected = new TsplDrawCommand("TEXT", new List<string> { "10", "10", "3", "0", "1", "1", "Hello Altec" });
        
        var result = _parser.Parse(tspl);

        Assert.Equal(expected.Name, result[0].Name);
        Assert.Equal(expected.Arguments, result[0].Arguments);
    }
}