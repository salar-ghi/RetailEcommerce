using Application.Helper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;
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
