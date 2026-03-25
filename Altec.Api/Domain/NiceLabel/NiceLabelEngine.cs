using NiceLabel.SDK;

namespace Altec.Api.Domain.NiceLabel;

public class NiceLabelEngine
{
    private readonly IPrintEngine _printEngine;

    public NiceLabelEngine()
    {
        PrintEngineFactory.SDKFilesPath = @"C:\Program Files\NiceLabel\NiceLabel 10\bin.net";
        _printEngine = PrintEngineFactory.PrintEngine;
        _printEngine.Initialize();
    }
}