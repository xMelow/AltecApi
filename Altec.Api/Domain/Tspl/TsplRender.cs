using Altec.Api.Records;
using SkiaSharp;

namespace Altec.Api.Domain.Tspl;

public class TsplRender
{
    private const double PrinterDpi = 300.0;
    private const double ScreenDpi = 96.0;
    
    public byte[] Render(IReadOnlyList<TsplDrawCommand> commands)
    {
        var sizeCommand = commands.FirstOrDefault(command => command.Name == "SIZE")
            ?? throw new InvalidOperationException("SIZE command is required");
        
        var width = Mm2Pixels(int.Parse(sizeCommand.Arguments[0]));
        var height = Mm2Pixels(int.Parse(sizeCommand.Arguments[1]));
        var bitmap = CreateBitMap(width, height, commands);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }

    private int Mm2Pixels(int mm)
    {
        var dots = (mm / 25.4) * PrinterDpi;
        return Convert.ToInt32(Dots2Pixels((int) dots));
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
                    DrawBarCommand(command, canvas);
                    break;
                case "BOX":
                    DrawBoxCommand(command, canvas);
                    break;
                case "CIRCLE":
                    DrawCircleCommand(command, canvas);
                    break;
                case "BARCODE":
                    DrawBarcodeCommand(command, canvas);
                    break;
                case "QRCODE":
                    DrawQrcodeCommand(command, canvas);
                    break;
                case "PUTBMP":
                    DrawBmpCommand(command, canvas);
                    break;
                case "BLOCK":
                    DrawBlockCommand(command, canvas);
                    break;
            }
        }

        return bitmap;
    }

    private float Dots2Pixels(int dots)
    {
        return Convert.ToSingle(dots * (ScreenDpi / PrinterDpi));
    }
    
    private void DrawTextCommand(TsplDrawCommand command, SKCanvas canvas)
    {        
        const double baseDotHeight = 3.6;
        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));
        var text = command.Arguments[6];
        var yScale = int.Parse(command.Arguments[5]);
        var fontSize = Dots2Pixels((int)(baseDotHeight * yScale));

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };

        using var font = new SKFont
        {
            Size = fontSize
        };
        
        var textBounds = new SKRect();
        paint.MeasureText(text, ref textBounds);
        var yBaseline = y + textBounds.Height;
        
        canvas.DrawText(text, x, yBaseline, font, paint);
    }
    
    private void DrawBarCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));
        var width = Dots2Pixels(int.Parse(command.Arguments[2]));
        var height = Dots2Pixels(int.Parse(command.Arguments[3]));

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill
        };
        
        canvas.DrawRect(x, y, width, height, paint);
    }
    
    private void DrawBoxCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));
        var xEnd = Dots2Pixels(int.Parse(command.Arguments[2]));
        var yEnd = Dots2Pixels(int.Parse(command.Arguments[3]));
        var width = xEnd - x;
        var height = yEnd - y;

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = Dots2Pixels(int.Parse(command.Arguments[4]))
        };
        
        canvas.DrawRect(x, y, width, height, paint);
    }
    
    private void DrawCircleCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));
        var diameter = Dots2Pixels(int.Parse(command.Arguments[2]));

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = Dots2Pixels(int.Parse(command.Arguments[3]))
        };
        
        canvas.DrawCircle(x, y, diameter / 2, paint);
    }

    private void DrawBlockCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        //todo: implement function
    }

    private void DrawBmpCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        //todo: implement function
    }

    private void DrawQrcodeCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        //todo: implement function
    }

    private void DrawBarcodeCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        //todo: implement function
    }
}