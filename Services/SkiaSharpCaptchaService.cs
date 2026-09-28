using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;
namespace ServiceAggregator.Api.Services;
public class SkiaSharpCaptchaService
{
    private readonly IMemoryCache _cache;

    public SkiaSharpCaptchaService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public (string Base64Image, string CaptchaKey, string Captcha) GenerateCaptchaImage(int width = 180, int height = 60)
    {
        Console.WriteLine("Start");
        var rand = new Random();
        string captchaText = GenerateRandomText(6);
        string captchaKey = Guid.NewGuid().ToString();

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        // خط‌های تصادفی
        using (var pen = new SKPaint
        {
            Color = SKColors.LightGray,
            StrokeWidth = 1
        })
        {
            for (int i = 0; i < 50; i++)
            {
                int x1 = rand.Next(width), y1 = rand.Next(height);
                int x2 = rand.Next(width), y2 = rand.Next(height);
                canvas.DrawLine(x1, y1, x2, y2, pen);
            }
        }

        // نویز نقطه‌ای
        for (int i = 0; i < 300; i++)
        {
            var paint = new SKPaint
            {
                Color = new SKColor((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)),
            };
            canvas.DrawPoint(rand.Next(width), rand.Next(height), paint);
        }

        using var font = new SKFont(SKTypeface.Default, 30);
        using var paintText = new SKPaint
        {
            IsAntialias = true,
            Color = SKColors.DarkSlateGray
        };

        for (int i = 0; i < captchaText.Length; i++)
        {
            var letter = captchaText[i].ToString();
            var rotation = rand.Next(-60, 60);

            canvas.Save();
            canvas.Translate(20 + i * 25, 40);
            canvas.RotateDegrees(rotation);
            paintText.Color = SKColors.Black.WithAlpha((byte)rand.Next(150, 255));
            canvas.DrawText(letter, 0, 0, SKTextAlign.Left, font, paintText);
            canvas.Restore();
        }

        // تبدیل به base64
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var base64 = Convert.ToBase64String(data.ToArray());

        _cache.Set(captchaKey, captchaText, TimeSpan.FromMinutes(2));
        return ($"data:image/png;base64,{base64}", captchaKey, captchaText);
    }

    public bool ValidateCaptcha(string? key, string? userInput)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(userInput))
            return false;
        if (_cache.TryGetValue(key, out string? correctValue))
        {
            return string.Equals(correctValue, userInput, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    private string GenerateRandomText(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ1234567890abcdefghjkmnpqrstuvwxyz";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
