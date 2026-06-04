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
        var parsedImage = ParseBase64Image(dataUrl);
        if (parsedImage == null)
            return null;

        string folderPath = Path.Combine(_env.ContentRootPath, subFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var existingRelativePath = await FindExistingImagePathAsync(
            folderPath,
            subFolder,
            parsedImage.Bytes,
            parsedImage.Hash,
            parsedImage.Extension,
            imagePrefix);

        if (existingRelativePath != null)
            return existingRelativePath;

        var fileName = $"{imagePrefix}-{parsedImage.Hash}{parsedImage.Extension}";
        var filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(filePath))
        {
            await File.WriteAllBytesAsync(filePath, parsedImage.Bytes);
        }

        // return relative path for the client, e.g. "images/categories/xyz.jpg"
        return BuildRelativePath(subFolder, fileName);
    }

    public async Task<string> SaveBase64ImageIfChanged(
        string dataUrl,
        string? existingImageUrl,
        string subFolder,
        string imagePrefix)
    {
        var parsedImage = ParseBase64Image(dataUrl);
        if (parsedImage == null)
            return existingImageUrl;

        if (!string.IsNullOrWhiteSpace(existingImageUrl) &&
            await ImageMatchesExistingFileAsync(existingImageUrl, parsedImage.Bytes))
        {
            return existingImageUrl;
        }

        return await SaveBase64Image(dataUrl, subFolder, imagePrefix);
    }


    public Task DeleteImageAsync(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return Task.CompletedTask;

        var relativePath = imageUrl.TrimStart('/');
        var filePath = Path.GetFullPath(Path.Combine(_env.ContentRootPath, relativePath));
        var contentRootPath = Path.GetFullPath(_env.ContentRootPath);
        var pathComparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        var relativeToContentRoot = Path.GetRelativePath(contentRootPath, filePath);

        if (relativeToContentRoot == ".." ||
            relativeToContentRoot.StartsWith($"..{Path.DirectorySeparatorChar}", pathComparison) ||
            Path.IsPathRooted(relativeToContentRoot))
        {
            throw new ArgumentException("Image path must be inside the application content root.", nameof(imageUrl));
        }

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
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

    private static ParsedImage? ParseBase64Image(string dataUrl)
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

        return new ParsedImage(imageBytes, extension, ComputeHash(imageBytes));
    }

    private async Task<string?> FindExistingImagePathAsync(
        string folderPath,
        string subFolder,
        byte[] imageBytes,
        string hash,
        string extension,
        string imagePrefix)
    {
        var hashFileName = $"{imagePrefix}-{hash}{extension}";
        var hashFilePath = Path.Combine(folderPath, hashFileName);
        if (File.Exists(hashFilePath))
            return BuildRelativePath(subFolder, hashFileName);

        foreach (var filePath in Directory.EnumerateFiles(folderPath))
        {
            if (!SupportedImageMimeTypes.ContainsKey(Path.GetExtension(filePath)))
                continue;

            var existingBytes = await File.ReadAllBytesAsync(filePath);
            if (existingBytes.Length == imageBytes.Length &&
                existingBytes.SequenceEqual(imageBytes))
            {
                return BuildRelativePath(subFolder, Path.GetFileName(filePath));
            }
        }

        return null;
    }

    private async Task<bool> ImageMatchesExistingFileAsync(string existingImageUrl, byte[] imageBytes)
    {
        var relativePath = existingImageUrl.TrimStart('/');
        var filePath = Path.Combine(_env.ContentRootPath, relativePath);

        if (!File.Exists(filePath))
            return false;

        var existingBytes = await File.ReadAllBytesAsync(filePath);
        return existingBytes.Length == imageBytes.Length && existingBytes.SequenceEqual(imageBytes);
    }

    private static string ComputeHash(byte[] bytes)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(bytes)).ToLowerInvariant();
    }

    private static string BuildRelativePath(string subFolder, string fileName)
    {
        return Path.Combine(subFolder, fileName).Replace("\\", "/");
    }

    private sealed record ParsedImage(byte[] Bytes, string Extension, string Hash);
}
