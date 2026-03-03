using Altec.Api.Domain.Tspl;
using Altec.Api.Records;
using Xunit.Abstractions;

namespace Altec.Api.Test.Domain.Tspl;

public class TsplRenderTest
{
    private readonly TsplRender _render = new();
    private readonly ITestOutputHelper _output;

    public TsplRenderTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void Render_TextCommand_SavesToPng()
    {
        var commands = new List<TsplDrawCommand>
        {
            new TsplDrawCommand("SIZE", new List<string> { "103", "110" }),
            new TsplDrawCommand("TEXT", new List<string> { "10", "10", "0", "0", "16", "16", "Hello Altec" })
        };
    
        var result = _render.Render(commands);
    
        File.WriteAllBytes("C:/temp/test_label.png", result);
        Assert.NotEmpty(result);
    }
}