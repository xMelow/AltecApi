using System.Runtime.CompilerServices;
using Altec.Api.Records;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;
using ZXing.SkiaSharp.Rendering;

namespace Altec.Api.Domain.Tspl;

public class TsplRender
{
    private const double PrinterDpi = 300.0;
    private const double ScreenDpi = 96.0;
    
    public byte[] Render(IReadOnlyList<TsplDrawCommand> commands, bool showBlockOutline, Dictionary<string, string> images)
    {
        var sizeCommand = commands.FirstOrDefault(command => command.Name == "SIZE")
            ?? throw new InvalidOperationException("SIZE command is required");
        
        var width = Mm2Pixels(int.Parse(sizeCommand.Arguments[0]));
        var height = Mm2Pixels(int.Parse(sizeCommand.Arguments[1]));

        return CreateBitMap(width, height, commands, showBlockOutline, images);
    }

    private int Mm2Pixels(int mm)
    {
        var dots = (mm / 25.4) * PrinterDpi;
        return Convert.ToInt32(Dots2Pixels((int) dots));
    }
    
    private byte[] CreateBitMap(int width, int height, IReadOnlyList<TsplDrawCommand> commands, bool showBlockOutline, Dictionary<string, string> images)
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
                    DrawBmpCommand(command, canvas, images);
                    break;
                case "BLOCK":
                    DrawBlockCommand(command, canvas, showBlockOutline);
                    break;
            }
        }
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        
        return data.ToArray();
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
        var radius = diameter / 2;
        var centerX = x + radius;
        var centerY = y + radius;

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = Dots2Pixels(int.Parse(command.Arguments[3]))
        };
        
        canvas.DrawCircle(centerX, centerY, diameter / 2, paint);
    }

    private void DrawBlockCommand(TsplDrawCommand command, SKCanvas canvas, bool showBlockOutline)
    {
        var (x, y, width, height, fontSize, align, text) = ParseBlockArguments(command);
    
        using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        using var font = new SKFont { Size = fontSize };
        
        DrawWrappedText(canvas, font, paint, text, x, y, width, fontSize, align);
        
        if (showBlockOutline)
            DrawBlockOutline(canvas, x, y, width, height);
    }

    private (float x, float y, float width, float height, float fontSize, int algin, string text) ParseBlockArguments(TsplDrawCommand command)
    {
        const double baseDotHeight = 3.6;
        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));
        var width = Dots2Pixels(int.Parse(command.Arguments[2]));
        var height = Dots2Pixels(int.Parse(command.Arguments[3]));
        var yScale = int.Parse(command.Arguments[7]);
        var text = command.Arguments[^1];
        var fontSize = Dots2Pixels((int)(baseDotHeight * yScale));
        var align = command.Arguments.Count >= 11
            ? int.Parse(command.Arguments[9])
            : 0;
        
        return (x, y, width, height, fontSize, align, text);
    } 

    private void DrawWrappedText(SKCanvas canvas, SKFont font, SKPaint paint, string text, float x, float y,
        float blockWidth, float fontSize, int align)
    {
        var currentLine = "";
        font.GetFontMetrics(out var metrics);
        var yCursor = y + Math.Abs(metrics.Ascent) * 0.5f;
        
        foreach (var word in text.Split(" "))
        {
            if (font.MeasureText(currentLine + word) >= blockWidth)
            {
                var drawX = align == 2
                    ? x + (blockWidth - font.MeasureText(currentLine)) / 2
                    : x;
                canvas.DrawText(currentLine, drawX, yCursor, font, paint);
                yCursor += fontSize * 1.2f;
                currentLine = word + " ";
            }
            else
                currentLine += word + " ";
        }
        if (!string.IsNullOrEmpty(currentLine))
        {
            var drawX = align == 2
                ? x + (blockWidth - font.MeasureText(currentLine)) / 2
                : x;
            canvas.DrawText(currentLine, drawX, yCursor, font, paint);
        }
    }

    private void DrawBlockOutline(SKCanvas canvas, float x, float y, float blockWidth, float blockHeight)
    {
        using var outlinePaint = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };
        canvas.DrawRect(x, y, blockWidth, blockHeight, outlinePaint);
    }
    
    private void DrawQrcodeCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        const int qrGridCells = 25;
        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));
        var content = command.Arguments[^1];
        var cellWidth = int.Parse(command.Arguments[3]);
        var size = Dots2Pixels(cellWidth * qrGridCells);

        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Width = (int)size,
                Height = (int)size,
                Margin = 1
            },
            Renderer = new SKBitmapRenderer()
        };
        var qrBitmap = writer.Write(content);
        canvas.DrawBitmap(qrBitmap, x, y);
    }
    
    private void DrawBarcodeCommand(TsplDrawCommand command, SKCanvas canvas)
    {
        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));
        var height = Dots2Pixels(int.Parse(command.Arguments[3]));
        var narrow = Dots2Pixels(int.Parse(command.Arguments[6]));
        var width = narrow * 100;
        var content = command.Arguments[^1];
        var barcodeType = command.Arguments[2];
        var barcodeFormat = barcodeType switch
        {
            "128" => BarcodeFormat.CODE_128,
            "39" => BarcodeFormat.CODE_39,
            "EAN13" => BarcodeFormat.EAN_13,
            "EAN8" => BarcodeFormat.EAN_8,
            _ => BarcodeFormat.CODE_128
        };
        
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = barcodeFormat,
            Options = new EncodingOptions
            {
                Width = (int)width,
                Height = (int)height,
                Margin = 1,
            },
            Renderer = new SKBitmapRenderer()
        };
        var barcodeBitMap = writer.Write(content);
        canvas.DrawBitmap(barcodeBitMap, x, y);
    }
    
    private void DrawBmpCommand(TsplDrawCommand command, SKCanvas canvas, Dictionary<string, string> images)
    {
        var filename = command.Arguments[2].ToLower();
        if (!images.ContainsKey(filename)) return;

        var x = Dots2Pixels(int.Parse(command.Arguments[0]));
        var y = Dots2Pixels(int.Parse(command.Arguments[1]));

        var imageBytes = Convert.FromBase64String(images[filename]);
        var bitmap = SKBitmap.Decode(imageBytes);

        var scaledWidth = (int)(bitmap.Width * (ScreenDpi / PrinterDpi));
        var scaledHeight = (int)(bitmap.Height * (ScreenDpi / PrinterDpi));

        var scaledBitmap = bitmap.Resize(new SKImageInfo(scaledWidth, scaledHeight), SKFilterQuality.High);
    
        canvas.DrawBitmap(scaledBitmap, x, y);
    }
}