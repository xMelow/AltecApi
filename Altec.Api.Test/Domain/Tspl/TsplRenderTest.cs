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
            new("SIZE", new List<string> { "103", "110" }),
            new("TEXT", new List<string> { "10", "10", "0", "0", "16", "16", "Hello Altec" })
        };
    
        var result = _render.Render(commands, false);
    
        File.WriteAllBytes("C:/temp/test_label.png", result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Render_FullTsplLabel_SavesToPng()
    {
        var commands = new List<TsplDrawCommand>
        {
            new("SIZE", new List<string> { "103", "103" }),
            new("TEXT", new List<string> { "427", "47", "0", "0", "16", "16", "Hello World" }),
            new("BAR", new List<string> { "33", "135", "1199", "7" }),
            new("BOX", new List<string> { "33", "178", "1232", "890", "6" }),
            new("QRCODE", new List<string> { "938", "951", "L", "14", "A", "0", "M2", "S7", "123456789012" }),
            new("CIRCLE", new List<string> { "933", "590", "260", "12" }),
            new("TEXT", new List<string> { "77", "234", "0", "0", "16", "16", "Name:" }),
            new("TEXT", new List<string> { "77", "322", "0", "0", "16", "16", "Phone number:" }),
            new("TEXT", new List<string> { "77", "413", "0", "0", "16", "16", "Email:" }),
            new("TEXT", new List<string> { "77", "505", "0", "0", "16", "16", "Company:" }),
            new("TEXT", new List<string> { "295", "231", "0", "0", "16", "16", "flor" }),
            new("TEXT", new List<string> { "509", "320", "0", "0", "16", "16", "+32468294226" }),
            new("TEXT", new List<string> { "295", "410", "0", "0", "16", "16", "flor@altec.be" }),
            new("TEXT", new List<string> { "396", "502", "0", "0", "16", "16", "Altec" }),
        };

        var result = _render.Render(commands, false);
        
        File.WriteAllBytes("C:/temp/testLabel.png", result);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public void Render_FullComplicatedTsplLabel_SavesToPng()
    {
        var commands = new List<TsplDrawCommand>
        {
            new("SIZE", new List<string> { "80", "101" }),
            new("BLOCK", new List<string> { "45", "300", "800", "60", "0", "0", "50", "60", "0", "2", "100" }),
            new("BLOCK", new List<string> { "32", "550", "800", "50", "0", "0", "26", "26", "Altec" }),
            new("BLOCK", new List<string> { "32", "680", "800", "250", "0", "0", "26", "26", "0", "1", "1", "Poduim boom altec"}),
            new("TEXT", new List<string> { "32", "960", "0", "0", "12", "14", "DATUM IN:" }),
            new("BLOCK", new List<string> { "32", "1025", "800", "50", "0", "0", "22", "22", "01/01/2026" }),
            new("QRCODE", new List<string> { "700", "900", "L", "10", "A", "0", "M2", "S7", "132465789" }),
        };

        var result = _render.Render(commands, true);
        
        File.WriteAllBytes("C:/temp/testLabel.png", result);
        Assert.NotEmpty(result);
    }
}