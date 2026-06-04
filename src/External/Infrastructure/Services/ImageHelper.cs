namespace Infrastructure.Services;

public class ImageHelper : IImageHelper
{
    private static readonly IReadOnlyDictionary<string, string> SupportedImageExtensions =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/avif"] = ".avif",
            ["image/bmp"] = ".bmp",
            ["image/gif"] = ".gif",
            ["image/heic"] = ".heic",
            ["image/heif"] = ".heif",
            ["image/jpg"] = ".jpg",
            ["image/jpeg"] = ".jpg",
            ["image/pjpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/svg+xml"] = ".svg",
            ["image/tiff"] = ".tiff",
            ["image/vnd.microsoft.icon"] = ".ico",
            ["image/webp"] = ".webp",
            ["image/x-icon"] = ".ico",
            ["image/x-png"] = ".png",
        };

    private static readonly IReadOnlyDictionary<string, string> SupportedImageMimeTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".avif"] = "image/avif",
            [".bmp"] = "image/bmp",
            [".gif"] = "image/gif",
            [".heic"] = "image/heic",
            [".heif"] = "image/heif",
            [".ico"] = "image/x-icon",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".svg"] = "image/svg+xml",
            [".tif"] = "image/tiff",
            [".tiff"] = "image/tiff",
            [".webp"] = "image/webp",
        };

    private readonly IWebHostEnvironment _env;

    public ImageHelper(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveBase64Image(string dataUrl, string subFolder, string imagePrefix)
    {
        if (string.IsNullOrWhiteSpace(dataUrl))
            return null;

        // data:[mime];base64,xxx
        var match = Regex.Match(
            dataUrl.Trim(),
            @"^data:(?<mime>image/[a-z0-9.+-]+);base64,(?<data>.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!match.Success)
        {
            throw new ArgumentException(
                "Invalid image data URL. Expected format: data:image/{type};base64,{data}.");
        }

        var mimeType = match.Groups["mime"].Value.ToLowerInvariant();
        var base64 = match.Groups["data"].Value;

        if (!SupportedImageExtensions.TryGetValue(mimeType, out var extension))
        {
            throw new ArgumentException(
                $"Unsupported image type '{mimeType}'. Supported image types are: {string.Join(", ", SupportedImageExtensions.Keys.OrderBy(type => type))}.");
        }

        byte[] imageBytes;
        try
        {
            imageBytes = Convert.FromBase64String(base64);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid base64 image content.", ex);
        }

        var fileName = $"{imagePrefix}-{Guid.NewGuid()}{extension}";
        string folderPath = Path.Combine(_env.ContentRootPath, subFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, fileName);

        await File.WriteAllBytesAsync(filePath, imageBytes);

        // return relative path for the client, e.g. "images/categories/xyz.jpg"
        var relativePath = Path.Combine(subFolder, fileName).Replace("\\", "/");
        return relativePath;
    }

    public async Task<string> GetImageBase64(string imageUrl)
    {
        string relativePath = imageUrl.TrimStart('/');
        string filePath = Path.Combine(_env.ContentRootPath, relativePath);

        if (!File.Exists(filePath))
            return null;

        var bytes = await File.ReadAllBytesAsync(filePath);
        var base64 = Convert.ToBase64String(bytes);

        // choose correct mime based on extension
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var mime = SupportedImageMimeTypes.GetValueOrDefault(ext, "application/octet-stream");

        return $"data:{mime};base64,{base64}";
    }
}
