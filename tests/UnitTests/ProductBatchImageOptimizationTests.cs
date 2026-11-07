using Application.DTOs;
using Application.Helper;
using Application.Mapping;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace UnitTests;

public class ProductBatchImageOptimizationTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly Mock<IImageHelper> _mockImageHelper;
    private readonly ProductService _productService;

    public ProductBatchImageOptimizationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        _unitOfWork = new UnitOfWork(_dbContext, mockCache.Object);

        var config = new TypeAdapterConfig();
        new ProductMappingProfile().Register(config);
        var mapper = new ServiceMapper(null!, config);

        _mockImageHelper = new Mock<IImageHelper>();

        _mockImageHelper
            .Setup(x => x.GetImagesBase64Async(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync((IEnumerable<string> urls) =>
            {
                var dict = new Dictionary<string, string>();
                foreach (var url in urls)
                {
                    dict[url] = $"data:image/webp;base64,{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url))}";
                }
                return dict;
            });

        _productService = new ProductService(_unitOfWork, mapper, _mockImageHelper.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_BatchesImageConversions_AcrossAllProducts()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Perfume", Description = "Perfume Category" };
        var brand = new Brand { Id = 1, Name = "Dior", Description = "Brand Dior" };
        _dbContext.Categories.Add(category);
        _dbContext.Brands.Add(brand);

        for (int i = 1; i <= 5; i++)
        {
            var product = new Product
            {
                Name = $"Product {i}",
                Description = $"Desc {i}",
                IsActive = true,
                IsDeleted = false,
                CategoryId = 1,
                BrandId = 1,
                StorageLocationNote = "",
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = $"images/p{i}_1.webp", IsPrimary = true },
                    new() { ImageUrl = $"images/p{i}_2.webp", IsPrimary = false }
                }
            };
            _dbContext.Products.Add(product);
        }
        await _dbContext.SaveChangesAsync();

        // Act
        var result = (await _productService.GetAllProductsAsync()).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        _mockImageHelper.Verify(x => x.GetImagesBase64Async(It.IsAny<IEnumerable<string>>()), Times.Once);
        Assert.All(result, p =>
        {
            Assert.NotNull(p.Images);
            Assert.Equal(2, p.Images.Count);
            Assert.StartsWith("data:image/webp;base64,", p.Images[0]);
        });
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
