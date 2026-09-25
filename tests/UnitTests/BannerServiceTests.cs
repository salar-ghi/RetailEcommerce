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

public class BannerServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly BannerService _bannerService;
    private readonly Mock<IImageHelper> _mockImageHelper;
    private readonly ITestOutputHelper _output;

    public BannerServiceTests(ITestOutputHelper output)
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
            cfg.AddProfile<BannerMappingProfile>();
        });
        var mapper = mapperConfig.CreateMapper();

        _mockImageHelper = new Mock<IImageHelper>();

        _bannerService = new BannerService(_unitOfWork, mapper, _mockImageHelper.Object);
    }

    [Fact]
    public async Task UpdateAsync_RemovesUnusedPlacements_UsingDeleteRangeAsync()
    {
        // Arrange
        var banner = new Banner
        {
            Id = 1,
            Name = "Test Banner",
            Description = "Desc",
            IsActive = true
        };
        _dbContext.Banners.Add(banner);

        for (int i = 1; i <= 10; i++)
        {
            _dbContext.BannerPlacement.Add(new BannerPlacement
            {
                Id = i,
                Name = $"Placement {i}"
            });

            _dbContext.Set<BannerPlacementMap>().Add(new BannerPlacementMap
            {
                Id = i,
                BannerId = 1,
                PlacementId = i,
                IsDeleted = false
            });
        }
        await _dbContext.SaveChangesAsync();

        var updateDto = new UpdateBannerDto
        {
            Id = 1,
            Name = "Updated Banner",
            PlacementIds = new List<int> { 1, 2 } // Placements 3 to 10 should be removed
        };

        // Act
        var sw = Stopwatch.StartNew();
        await _bannerService.UpdateAsync(updateDto);
        sw.Stop();

        _output.WriteLine($"UpdateAsync took: {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalMicroseconds} us)");

        // Assert
        var activeMaps = await _dbContext.Set<BannerPlacementMap>()
            .Where(m => m.BannerId == 1 && !m.IsDeleted)
            .ToListAsync();

        Assert.Equal(2, activeMaps.Count);
        Assert.Contains(activeMaps, m => m.PlacementId == 1);
        Assert.Contains(activeMaps, m => m.PlacementId == 2);
    }

    [Fact]
    public async Task CreateAsync_ThrowsKeyNotFoundException_WhenPlacementNotFound()
    {
        // Arrange
        var createDto = new CreateBannerDto
        {
            Name = "New Banner",
            PlacementIds = new List<int> { 999 } // Placement 999 does not exist
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _bannerService.CreateAsync(createDto));
        Assert.Equal("Some placements not found.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsKeyNotFoundException_WhenPlacementNotFound()
    {
        // Arrange
        var banner = new Banner
        {
            Id = 1,
            Name = "Test Banner",
            IsActive = true
        };
        _dbContext.Banners.Add(banner);
        await _dbContext.SaveChangesAsync();

        var updateDto = new UpdateBannerDto
        {
            Id = 1,
            Name = "Updated Banner",
            PlacementIds = new List<int> { 999 } // Placement 999 does not exist
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _bannerService.UpdateAsync(updateDto));
        Assert.Equal("Some placements not found.", ex.Message);
    }

    [Fact]
    public async Task Repository_DeleteRangeAsync_RemovesAllSpecifiedEntities()
    {
        // Arrange
        var repository = new Repository<BannerPlacementMap, int>(_dbContext);
        var maps = new List<BannerPlacementMap>();
        for (int i = 100; i < 200; i++)
        {
            maps.Add(new BannerPlacementMap
            {
                Id = i,
                BannerId = 1,
                PlacementId = i
            });
        }
        _dbContext.Set<BannerPlacementMap>().AddRange(maps);
        await _dbContext.SaveChangesAsync();

        Assert.Equal(100, await _dbContext.Set<BannerPlacementMap>().CountAsync(m => m.Id >= 100));

        // Act
        var sw = Stopwatch.StartNew();
        await repository.DeleteRangeAsync(maps);
        await _dbContext.SaveChangesAsync();
        sw.Stop();

        _output.WriteLine($"DeleteRangeAsync for 100 entities took: {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalMicroseconds} us)");

        // Assert
        Assert.Equal(0, await _dbContext.Set<BannerPlacementMap>().CountAsync(m => m.Id >= 100));
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
