using SixLabors.ImageSharp;
using System.Text.RegularExpressions;

namespace Presentation.Services;

public class ImageService
{
    private static readonly Regex DataUrlRegex = new(
        @"^data:(?<mime>image\/(jpeg|png|gif|webp));base64,(?<data>.+)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string?> SaveBase64Image(string? dataUrl, string subFolder)
    {
        if (string.IsNullOrWhiteSpace(dataUrl))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(subFolder))
        {
            throw new ArgumentException("Image subfolder is required.", nameof(subFolder));
        }

        var match = DataUrlRegex.Match(dataUrl);
        if (!match.Success)
        {
            throw new ArgumentException("Invalid image base64 format. Supported formats are jpeg, png, gif, and webp.", nameof(dataUrl));
        }

        var mimeType = match.Groups["mime"].Value.ToLowerInvariant();
        var base64 = match.Groups["data"].Value;

        var extension = mimeType switch
        {
            "image/webp" => ".webp",
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            _ => throw new ArgumentException("Unsupported image type.", nameof(dataUrl))
        };

        byte[] imageBytes;
        try
        {
            imageBytes = Convert.FromBase64String(base64);
        }
        catch (FormatException exception)
        {
            throw new ArgumentException("Invalid base64 string.", nameof(dataUrl), exception);
        }

        using var image = Image.Load(imageBytes);

        var fileName = $"category-{Guid.NewGuid()}{extension}";
        var folderPath = Path.Combine(_env.ContentRootPath, subFolder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        await File.WriteAllBytesAsync(filePath, imageBytes);

        return Path.Combine(subFolder, fileName).Replace("\\", "/");
    }

    public async Task<string?> GetImageBase64(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        var relativePath = imageUrl.TrimStart('/');
        var filePath = Path.Combine(_env.ContentRootPath, relativePath);

        if (!File.Exists(filePath))
        {
            return null;
        }

        var bytes = await File.ReadAllBytesAsync(filePath);
        var base64 = Convert.ToBase64String(bytes);

        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var mime = ext switch
        {
            ".webp" => "webp",
            ".jpg" => "jpeg",
            ".jpeg" => "jpeg",
            ".png" => "png",
            ".gif" => "gif",
            _ => "octet-stream"
        };

        return $"data:image/{mime};base64,{base64}";
    }
}
