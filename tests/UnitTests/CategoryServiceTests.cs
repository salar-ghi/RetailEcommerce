using System.Diagnostics;
using Application.DTOs;
using Application.Helper;
using Application.Interfaces;
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

public class CategoryServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly CategoryService _categoryService;
    private readonly Mock<IImageHelper> _mockImageHelper;
    private readonly ITestOutputHelper _output;

    public CategoryServiceTests(ITestOutputHelper output)
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
            cfg.AddProfile<CategoryMappingProfile>();
            cfg.AddProfile<BrandMappingProfile>();
        });
        var mapper = mapperConfig.CreateMapper();

        _mockImageHelper = new Mock<IImageHelper>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();

        _categoryService = new CategoryService(_unitOfWork, mockCurrentUserService.Object, mapper, _mockImageHelper.Object);
    }

    [Fact]
    public async Task GetCategoriesWithProductCount_ReturnsCorrectCounts()
    {
        // Seed 50 categories
        for (int i = 1; i <= 50; i++)
        {
            _dbContext.Categories.Add(new Category
            {
                Id = i,
                Name = $"Category {i}",
                Description = $"Description {i}",
                ParentId = i > 1 ? 1 : null,
                CreatedTime = DateTime.UtcNow
            });
        }

        // Seed products: Category c gets c * 2 active products + 1 inactive product
        for (int c = 1; c <= 50; c++)
        {
            int activeCount = (c % 5 == 0) ? 0 : c * 2; // Every 5th category has 0 active products
            for (int p = 1; p <= activeCount; p++)
            {
                _dbContext.Products.Add(new Product
                {
                    Name = $"Product {c}_{p}",
                    Description = "Desc",
                    CategoryId = c,
                    IsActive = true,
                    StorageLocationNote = ""
                });
            }

            // Inactive products (should NOT be counted)
            _dbContext.Products.Add(new Product
            {
                Name = $"Inactive Product {c}",
                Description = "Desc",
                CategoryId = c,
                IsActive = false,
                StorageLocationNote = ""
            });
        }

        await _dbContext.SaveChangesAsync();

        var sw = Stopwatch.StartNew();
        var result = await _categoryService.GetCategoriesWithProductCount();
        sw.Stop();

        _output.WriteLine($"GetCategoriesWithProductCount took: {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalMicroseconds} us)");

        Assert.Equal(50, result.Count);
        for (int c = 1; c <= 50; c++)
        {
            var catDto = result.FirstOrDefault(x => x.Id == c);
            Assert.NotNull(catDto);
            Assert.Equal($"Category {c}", catDto.Name);

            int expectedCount = (c % 5 == 0) ? 0 : c * 2;
            Assert.Equal(expectedCount, catDto.ProductCount);
        }
    }

    [Fact]
    public async Task GetAllCategoriesWithDetailsAsync_ReturnsCategoriesWithImages()
    {
        for (int i = 1; i <= 100; i++)
        {
            _dbContext.Categories.Add(new Category
            {
                Id = i,
                Name = $"Category {i:D3}",
                Description = $"Description {i}",
                ImageUrl = $"images/categories/cat_{i % 10}.png",
                IsDeleted = false,
                CreatedTime = DateTime.UtcNow
            });
        }
        await _dbContext.SaveChangesAsync();

        _mockImageHelper.Setup(x => x.GetImagesBase64Async(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync((IEnumerable<string> urls) =>
                urls.Distinct().ToDictionary(u => u, u => $"data:image/png;base64,dummy_{u}"));

        var sw = Stopwatch.StartNew();
        var result = await _categoryService.GetAllCategoriesWithDetailsAsync();
        sw.Stop();

        _output.WriteLine($"GetAllCategoriesWithDetailsAsync took: {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalMicroseconds} us)");

        Assert.Equal(100, result.Count);
        foreach (var cat in result)
        {
            Assert.NotNull(cat.Image);
            Assert.StartsWith("data:image/png;base64,dummy_", cat.Image);
        }
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
