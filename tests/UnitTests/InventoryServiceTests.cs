using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace UnitTests;

public class InventoryServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        _unitOfWork = new UnitOfWork(_dbContext, mockCache.Object);

        _inventoryService = new InventoryService(_unitOfWork);
    }

    [Theory]
    [InlineData("store_floor", "store_floor")]
    [InlineData("back_room", "back_room")]
    [InlineData("dark_store", "dark_store")]
    [InlineData("Warehouse", "warehouse")]
    [InlineData("Basement", "basement")]
    [InlineData("StoreFloor", "store_floor")]
    public async Task CreateSpaceAsync_WithValidType_ParsesAndFormatsTypeCorrectly(string inputType, string expectedType)
    {
        // Arrange
        var dto = new CreateStorageSpaceDto
        {
            Name = "Test Space",
            Type = inputType,
            Code = "TS-01",
            Address = "123 Test St",
            Description = "Test storage space",
            Capacity = 100
        };

        // Act
        var result = await _inventoryService.CreateSpaceAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test Space", result.Name);
        Assert.Equal(expectedType, result.Type);
        Assert.Equal("TS-01", result.Code);
        Assert.Equal("123 Test St", result.Address);
        Assert.Equal("Test storage space", result.Description);
        Assert.Equal(100, result.Capacity);
        Assert.Equal(0, result.Used);
        Assert.True(result.IsActive);

        var dbSpace = await _dbContext.StorageSpaces.FindAsync(result.Id);
        Assert.NotNull(dbSpace);
        Assert.Equal("Test Space", dbSpace.Name);
    }

    [Theory]
    [InlineData("invalid_type")]
    [InlineData("unknown_value")]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateSpaceAsync_WithInvalidOrUnknownType_FallsBackToOtherType(string? invalidType)
    {
        // Arrange
        var dto = new CreateStorageSpaceDto
        {
            Name = "Fallback Space",
            Type = invalidType!,
            Code = "FS-01",
            Address = "456 Fallback Rd",
            Description = "Space with unknown type",
            Capacity = 50
        };

        // Act
        var result = await _inventoryService.CreateSpaceAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("other", result.Type);

        var dbSpace = await _dbContext.StorageSpaces.FindAsync(result.Id);
        Assert.NotNull(dbSpace);
        Assert.Equal(StorageSpaceType.Other, dbSpace.Type);
    }

    [Fact]
    public async Task GetSpaceByIdAsync_WhenSpaceExists_ReturnsSpaceDto()
    {
        // Arrange
        var space = new StorageSpace
        {
            Name = "Existing Space",
            Type = StorageSpaceType.Warehouse,
            Code = "EX-01",
            Address = "789 Existing Way",
            Description = "Existing description",
            Capacity = 500,
            Used = 10,
            IsActive = true,
            IsDeleted = false
        };
        _dbContext.StorageSpaces.Add(space);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _inventoryService.GetSpaceByIdAsync(space.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(space.Id, result.Id);
        Assert.Equal("Existing Space", result.Name);
        Assert.Equal("warehouse", result.Type);
    }

    [Fact]
    public async Task GetSpaceByIdAsync_WhenSpaceDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _inventoryService.GetSpaceByIdAsync(999));
    }

    [Fact]
    public async Task UpdateSpaceAsync_WithValidDto_UpdatesSpaceInDb()
    {
        // Arrange
        var space = new StorageSpace
        {
            Name = "Old Name",
            Type = StorageSpaceType.StoreFloor,
            Code = "OLD-01",
            Address = "Old Address",
            Description = "Old Description",
            Capacity = 100,
            IsActive = true
        };
        _dbContext.StorageSpaces.Add(space);
        await _dbContext.SaveChangesAsync();

        var updateDto = new CreateStorageSpaceDto
        {
            Name = "New Name",
            Type = "back_room",
            Code = "NEW-01",
            Address = "Updated Address",
            Description = "Updated Description",
            Capacity = 200
        };

        // Act
        await _inventoryService.UpdateSpaceAsync(space.Id, updateDto);

        // Assert
        var updatedSpace = await _dbContext.StorageSpaces.FindAsync(space.Id);
        Assert.NotNull(updatedSpace);
        Assert.Equal("New Name", updatedSpace.Name);
        Assert.Equal(StorageSpaceType.BackRoom, updatedSpace.Type);
        Assert.Equal("NEW-01", updatedSpace.Code);
        Assert.Equal("Updated Address", updatedSpace.Address);
        Assert.Equal("Updated Description", updatedSpace.Description);
        Assert.Equal(200, updatedSpace.Capacity);
    }

    [Fact]
    public async Task UpdateSpaceAsync_WhenSpaceDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = new CreateStorageSpaceDto
        {
            Name = "Nonexistent Space",
            Type = "warehouse",
            Code = "NONE",
            Address = "Addr",
            Description = "Desc"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _inventoryService.UpdateSpaceAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteSpaceAsync_WhenSpaceExists_DeletesSpace()
    {
        // Arrange
        var space = new StorageSpace
        {
            Name = "Space to Delete",
            Type = StorageSpaceType.Other,
            Code = "DEL-01",
            Address = "Delete Address",
            Description = "Delete Description"
        };
        _dbContext.StorageSpaces.Add(space);
        await _dbContext.SaveChangesAsync();

        // Act
        await _inventoryService.DeleteSpaceAsync(space.Id);

        // Assert
        var deletedSpace = await _dbContext.StorageSpaces.FindAsync(space.Id);
        Assert.Null(deletedSpace);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
