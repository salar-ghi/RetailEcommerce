using Application.Helper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;
using SkiaSharp;
using Xunit;

namespace UnitTests;

public class ImageHelperTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly ImageHelper _imageHelper;

    public ImageHelperTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);

        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.ContentRootPath).Returns(_tempDirectory);

        _imageHelper = new ImageHelper(mockEnv.Object);
    }

    [Fact]
    public async Task GetImagesBase64Async_RetrievesMultipleImagesConcurrentlyAndDeduplicates()
    {
        // Create sample image files
        var file1Relative = "images/cat1.png";
        var file2Relative = "images/cat2.png";

        var file1Full = Path.Combine(_tempDirectory, file1Relative);
        var file2Full = Path.Combine(_tempDirectory, file2Relative);

        Directory.CreateDirectory(Path.GetDirectoryName(file1Full)!);

        byte[] content1 = [1, 2, 3, 4];
        byte[] content2 = [5, 6, 7, 8];

        await File.WriteAllBytesAsync(file1Full, content1);
        await File.WriteAllBytesAsync(file2Full, content2);

        var requestedUrls = new[]
        {
            file1Relative,
            file1Relative, // Duplicate
            file2Relative,
            "images/non_existent.png",
            "",
            null
        };

        var result = await _imageHelper.GetImagesBase64Async(requestedUrls!);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Only existing files returned
        Assert.True(result.ContainsKey(file1Relative));
        Assert.True(result.ContainsKey(file2Relative));
        Assert.StartsWith("data:image/png;base64,", result[file1Relative]);
        Assert.StartsWith("data:image/png;base64,", result[file2Relative]);
    }

    [Fact]
    public async Task SaveBase64Image_ConvertsPngToWebp()
    {
        // 1x1 valid PNG base64
        var pngBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAACklEQVR4nGNgAAAAAgAB4iG8MwAAAABJRU5ErkJggg==";

        var relativePath = await _imageHelper.SaveBase64Image(pngBase64, "images/products", "product");

        Assert.NotNull(relativePath);
        Assert.EndsWith(".webp", relativePath);
        Assert.StartsWith("images/products/product-", relativePath);

        var fullPath = Path.Combine(_tempDirectory, relativePath);
        Assert.True(File.Exists(fullPath));

        var savedBytes = await File.ReadAllBytesAsync(fullPath);
        using var codec = SKCodec.Create(new MemoryStream(savedBytes));
        Assert.NotNull(codec);
        Assert.Equal(SKEncodedImageFormat.Webp, codec.EncodedFormat);
    }

    [Fact]
    public async Task SaveBase64Image_PreservesExistingWebpWithoutReencoding()
    {
        // Valid 1x1 WebP
        byte[] webpBytes;
        using (var bitmap = new SKBitmap(1, 1))
        using (var image = SKImage.FromBitmap(bitmap))
        using (var data = image.Encode(SKEncodedImageFormat.Webp, 100))
        {
            webpBytes = data.ToArray();
        }

        var webpBase64 = $"data:image/webp;base64,{Convert.ToBase64String(webpBytes)}";

        var relativePath = await _imageHelper.SaveBase64Image(webpBase64, "images/products", "product");

        Assert.NotNull(relativePath);
        Assert.EndsWith(".webp", relativePath);

        var fullPath = Path.Combine(_tempDirectory, relativePath);
        Assert.True(File.Exists(fullPath));

        var savedBytes = await File.ReadAllBytesAsync(fullPath);
        Assert.Equal(webpBytes, savedBytes);
    }

    [Fact]
    public async Task SaveBase64Image_InvalidRasterData_ThrowsArgumentException()
    {
        // Corrupted payload with valid MIME header
        var invalidDataUrl = "data:image/png;base64,AQIDBAUGBwg=";

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _imageHelper.SaveBase64Image(invalidDataUrl, "images/products", "product"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            try
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
