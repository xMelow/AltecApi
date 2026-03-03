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

    private int Dots2Pixels(int dots)
    {
        // screen dpi = 96.0
        // dpi = printer dpi = 300
        return Convert.ToInt32(dots * (Dpi / 96.0));
    }
    
    private void DrawTextCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));
        var text = command.Arguments[6];
        var fontSize = int.Parse(command.Arguments[4]) * (Dpi / 203.0f);

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };

        using var font = new SKFont
        {
            Size = fontSize
        };
        
        canvas.DrawText(text, x, y, font, paint);
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