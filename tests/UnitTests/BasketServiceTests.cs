using Application.Configuration;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.IRepositories;
using Moq;
using Xunit;

namespace UnitTests;

public class BasketServiceTests
{
    private readonly Mock<IProductRepository> _mockProducts;
    private readonly Mock<IRedisCacheService> _mockCache;
    private readonly BasketService _basketService;

    public BasketServiceTests()
    {
        _mockProducts = new Mock<IProductRepository>();
        _mockCache = new Mock<IRedisCacheService>();
        _basketService = new BasketService(_mockProducts.Object, _mockCache.Object);
    }

    [Fact]
    public async Task AddItemToBasketAsync_ThrowsKeyNotFoundException_WhenProductNotFound()
    {
        // Arrange
        const string ownerId = "user-123";
        const long productId = 999L;
        const int quantity = 2;

        _mockProducts
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
            .ReturnsAsync((Product)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _basketService.AddItemToBasketAsync(ownerId, productId, quantity));

        Assert.Equal($"Product with ID {productId} was not found.", exception.Message);
        _mockCache.Verify(c => c.SetCachedDataAsync(It.IsAny<string>(), It.IsAny<BasketDto>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddItemToBasketAsync_ThrowsArgumentException_WhenOwnerIdIsInvalid(string? invalidOwnerId)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _basketService.AddItemToBasketAsync(invalidOwnerId!, 1L, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task AddItemToBasketAsync_ThrowsArgumentOutOfRangeException_WhenQuantityIsZeroOrNegative(int invalidQuantity)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _basketService.AddItemToBasketAsync("user-123", 1L, invalidQuantity));
    }

    [Fact]
    public async Task AddItemToBasketAsync_AddsNewItem_WhenProductExistsAndBasketIsEmpty()
    {
        // Arrange
        const string ownerId = "user-123";
        const long productId = 1L;
        const int quantity = 2;
        const decimal price = 25.50m;
        const string imageUrl = "http://example.com/image.jpg";

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Batches = new List<ProductInventoryBatch>
            {
                new() { SellingPrice = price }
            },
            Images = new List<ProductImage>
            {
                new() { Id = 1, ImageUrl = imageUrl, IsPrimary = true }
            }
        };

        _mockProducts
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
            .ReturnsAsync(product);

        _mockCache
            .Setup(c => c.GetCachedDataAsync<BasketDto>($"basket:{ownerId}"))
            .ReturnsAsync((BasketDto)null!);

        // Act
        var result = await _basketService.AddItemToBasketAsync(ownerId, productId, quantity);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        var item = result.Items[0];
        Assert.Equal(productId, item.ProductId);
        Assert.Equal("Test Product", item.ProductName);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(price, item.UnitPrice);
        Assert.Equal(imageUrl, item.CoverImage);
        Assert.Equal(imageUrl, item.imageUrl);
        Assert.Equal(quantity, result.TotalItems);
        Assert.Equal(quantity * price, result.TotalPrice);

        _mockCache.Verify(c => c.SetCachedDataAsync($"basket:{ownerId}", It.IsAny<BasketDto>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task AddItemToBasketAsync_UpdatesQuantity_WhenItemAlreadyInBasket()
    {
        // Arrange
        const string ownerId = "user-123";
        const long productId = 1L;
        const int initialQuantity = 2;
        const int addedQuantity = 3;
        const decimal price = 10.00m;

        var existingBasket = new BasketDto
        {
            Id = "basket-id",
            UserId = ownerId,
            GuestId = ownerId,
            Items = new List<BasketItemDto>
            {
                new()
                {
                    Id = productId,
                    ProductId = productId,
                    ProductName = "Old Product Name",
                    Quantity = initialQuantity,
                    UnitPrice = price
                }
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Updated Product Name",
            Batches = new List<ProductInventoryBatch>
            {
                new() { SellingPrice = price }
            },
            Images = new List<ProductImage>()
        };

        _mockProducts
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
            .ReturnsAsync(product);

        _mockCache
            .Setup(c => c.GetCachedDataAsync<BasketDto>($"basket:{ownerId}"))
            .ReturnsAsync(existingBasket);

        // Act
        var result = await _basketService.AddItemToBasketAsync(ownerId, productId, addedQuantity);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        var item = result.Items[0];
        Assert.Equal(initialQuantity + addedQuantity, item.Quantity);
        Assert.Equal("Updated Product Name", item.ProductName);
        Assert.Equal(initialQuantity + addedQuantity, result.TotalItems);
        Assert.Equal((initialQuantity + addedQuantity) * price, result.TotalPrice);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_ThrowsKeyNotFoundException_WhenItemNotInBasket()
    {
        // Arrange
        const string ownerId = "user-123";
        const long productId = 99L;

        var existingBasket = new BasketDto
        {
            Id = "basket-id",
            UserId = ownerId,
            GuestId = ownerId,
            Items = new List<BasketItemDto>()
        };

        _mockCache
            .Setup(c => c.GetCachedDataAsync<BasketDto>($"basket:{ownerId}"))
            .ReturnsAsync(existingBasket);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _basketService.UpdateItemQuantityAsync(ownerId, productId, 5));

        Assert.Equal($"Item with Product ID {productId} was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_RemovesItem_WhenQuantityIsZero()
    {
        // Arrange
        const string ownerId = "user-123";
        const long productId = 1L;

        var existingBasket = new BasketDto
        {
            Id = "basket-id",
            UserId = ownerId,
            GuestId = ownerId,
            Items = new List<BasketItemDto>
            {
                new() { Id = productId, ProductId = productId, Quantity = 2, UnitPrice = 15m }
            }
        };

        _mockCache
            .Setup(c => c.GetCachedDataAsync<BasketDto>($"basket:{ownerId}"))
            .ReturnsAsync(existingBasket);

        // Act
        var result = await _basketService.UpdateItemQuantityAsync(ownerId, productId, 0);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0m, result.TotalPrice);
    }
}
