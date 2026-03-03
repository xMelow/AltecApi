using Altec.Api.Records;
using SkiaSharp;

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
        var bitmap = CreateBitMap(width, height, commands);

        return bitmap.Bytes;
    }

    private int Mm2Pixels(int dots)
    {
        return Convert.ToInt32(Math.Round((dots / 25.4) * Dpi));
    }
    
    private SKBitmap CreateBitMap(int width, int height, IReadOnlyList<TsplDrawCommand> commands)
    {
        SKBitmap bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        foreach (var command in commands)
        {
            switch (command.Name)
            {
                case "TEXT":
                    DrawTextCommand(command, canvas);
                    break;
                case "BAR":
                    DrawBarCommand(command);
                    break;
                case "BOX":
                    DrawBoxCommand(command);
                    break;
                case "CIRCLE":
                    DrawCircleCommand(command);
                    break;
                case "BARCODE":
                    DrawBarcodeCommand(command);
                    break;
                case "QRCODE":
                    DrawQrcodeCommand(command);
                    break;
                case "PUTBMP":
                    DrawBmpCommand(command);
                    break;
                case "BLOCK":
                    DrawBlockCommand(command);
                    break;
            }
        }

        return bitmap;
    }
    
    private void DrawTextCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        throw new NotImplementedException();
    }

    private void DrawBlockCommand(TsplDrawCommand command)
    {
        throw new NotImplementedException();
    }

    private void DrawBmpCommand(TsplDrawCommand command)
    {
        throw new NotImplementedException();
    }

    private void DrawQrcodeCommand(TsplDrawCommand command)
    {
        throw new NotImplementedException();
    }

    private void DrawBarcodeCommand(TsplDrawCommand command)
    {
        throw new NotImplementedException();
    }

    private void DrawCircleCommand(TsplDrawCommand command)
    {
        throw new NotImplementedException();
    }

    private void DrawBoxCommand(TsplDrawCommand command)
    {
        throw new NotImplementedException();
    }

    private void DrawBarCommand(TsplDrawCommand command)
    {
        throw new NotImplementedException();
    }
}