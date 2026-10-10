using System.Diagnostics;
using Application.DTOs;
using Application.Helper;
using Application.Mapping;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests;

public class ProductServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly ProductService _productService;
    private readonly Mock<IImageHelper> _mockImageHelper;
    private readonly ITestOutputHelper _output;

    public ProductServiceTests(ITestOutputHelper output)
    {
        _output = output;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        _unitOfWork = new UnitOfWork(_dbContext, mockCache.Object);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
        });
        var mapper = mapperConfig.CreateMapper();

        _mockImageHelper = new Mock<IImageHelper>();

        // Simulate I/O delay for SaveBase64Image (e.g. 50ms per image)
        _mockImageHelper
            .Setup(x => x.SaveBase64Image(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string, string>(async (dataUrl, subFolder, prefix) =>
            {
                await Task.Delay(50);
                return $"/uploads/{subFolder}/{prefix}_{Guid.NewGuid():N}.png";
            });

        _productService = new ProductService(_unitOfWork, mapper, _mockImageHelper.Object);
    }

    [Fact]
    public async Task AddProductAsync_SavesMultipleImagesConcurrently_Benchmark()
    {
        // Arrange
        var coverImage = "data:image/png;base64,cover_data";
        var images = new List<string>
        {
            coverImage,
            "data:image/png;base64,image1_data",
            "data:image/png;base64,image2_data",
            "data:image/png;base64,image3_data",
            "data:image/png;base64,image4_data"
        };

        var contentBlocks = new List<ProductContentBlockDto>
        {
            new ProductContentBlockDto { Type = "paragraph", Text = "Intro text" },
            new ProductContentBlockDto { Type = "image", Image = "data:image/png;base64,block1_data", Caption = "Block 1" },
            new ProductContentBlockDto { Type = "image", Image = "data:image/png;base64,block2_data", Caption = "Block 2" }
        };

        var request = new CreateProductRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            CoverImage = coverImage,
            Images = images,
            ContentBlocks = contentBlocks,
            SupplierId = 1,
            Location = "Warehouse A"
        };

        // Act
        var sw = Stopwatch.StartNew();
        var result = await _productService.AddProductAsync(request);
        sw.Stop();

        _output.WriteLine($"AddProductAsync took: {sw.ElapsedMilliseconds} ms for {images.Count + 2} images");

        // Assert
        Assert.NotNull(result);
        var productInDb = await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Images)
            .Include(p => p.ContentBlocks)
            .FirstOrDefaultAsync(p => p.Name == "Test Product");

        Assert.NotNull(productInDb);
        Assert.Equal(5, productInDb.Images.Count);
        Assert.Single(productInDb.Images, i => i.IsPrimary);
        Assert.Equal(3, productInDb.ContentBlocks.Count);
        Assert.NotNull(productInDb.ContentBlocks.First(b => b.Type == "image").ImageUrl);
    }

    [Fact]
    public async Task UpdateProductAsync_SavesMultipleImagesConcurrently_Benchmark()
    {
        // Arrange
        var existingProduct = new Product
        {
            Id = 10,
            Name = "Existing Product",
            Description = "Old Description",
            StorageLocationNote = "Warehouse A",
            IsActive = true
        };
        _dbContext.Products.Add(existingProduct);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var coverImage = "data:image/png;base64,updated_cover";
        var images = new List<string>
        {
            coverImage,
            "data:image/png;base64,updated_image1",
            "data:image/png;base64,updated_image2",
            "data:image/png;base64,updated_image3",
            "data:image/png;base64,updated_image4"
        };

        var contentBlocks = new List<ProductContentBlockDto>
        {
            new ProductContentBlockDto { Type = "image", Image = "data:image/png;base64,updated_block1" },
            new ProductContentBlockDto { Type = "image", Image = "data:image/png;base64,updated_block2" }
        };

        var request = new UpdateProductRequest
        {
            Name = "Updated Product Name",
            Description = "Updated Description",
            CoverImage = coverImage,
            Images = images,
            ContentBlocks = contentBlocks,
            SupplierId = 1,
            Location = "Warehouse B"
        };

        // Act
        var sw = Stopwatch.StartNew();
        var result = await _productService.UpdateProductAsync(10, request);
        sw.Stop();

        _output.WriteLine($"UpdateProductAsync took: {sw.ElapsedMilliseconds} ms for {images.Count + 2} images");

        // Assert
        Assert.NotNull(result);
        var productInDb = await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Images)
            .Include(p => p.ContentBlocks)
            .FirstOrDefaultAsync(p => p.Id == 10);

        Assert.NotNull(productInDb);
        Assert.Equal(5, productInDb.Images.Count);
        Assert.Single(productInDb.Images, i => i.IsPrimary);
        Assert.Equal(2, productInDb.ContentBlocks.Count);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
