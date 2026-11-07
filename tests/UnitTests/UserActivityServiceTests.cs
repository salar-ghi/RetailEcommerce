using Application.DTOs;
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

public class UserActivityServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly UserActivityService _service;

    public UserActivityServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        _unitOfWork = new UnitOfWork(_dbContext, mockCache.Object);

        var config = new TypeAdapterConfig();
        new UserActivityMappingProfile().Register(config);
        var mapper = new ServiceMapper(null!, config);

        _service = new UserActivityService(_unitOfWork, mapper);
    }

    [Fact]
    public async Task TrackActivityAsync_ValidInput_PersistsAndReturnsDto()
    {
        var dto = new TrackUserActivityDto
        {
            Platform = "bazaar-azad",
            Action = "product_view",
            UserId = "user-123",
            SessionId = "sess-abc",
            EntityType = "Product",
            EntityId = "42",
            MetadataJson = "{\"category\":\"perfume\",\"brand\":\"chanel\"}"
        };

        var result = await _service.TrackActivityAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("product_view", result.Action);
        Assert.Equal("bazaar-azad", result.Platform);
        Assert.Equal("user-123", result.UserId);
        Assert.Equal("42", result.EntityId);

        var persisted = await _dbContext.UserActivities.FirstOrDefaultAsync(x => x.UserId == "user-123");
        Assert.NotNull(persisted);
        Assert.Equal("Product", persisted.EntityType);
    }

    [Fact]
    public async Task TrackBatchActivitiesAsync_MultipleActivities_SavesAllSuccessfully()
    {
        var batch = new BatchTrackUserActivityDto
        {
            Activities = new List<TrackUserActivityDto>
            {
                new() { Platform = "bazaar-azad", Action = "search", EntityId = "iphone 15", UserId = "u1" },
                new() { Platform = "bazaar-azad", Action = "product_view", EntityType = "Product", EntityId = "101", UserId = "u1" },
                new() { Platform = "store-panel", Action = "stock_update", EntityType = "Product", EntityId = "101", UserId = "admin-1" },
                new() { Platform = "bazaar-azad", Action = "add_to_cart", EntityType = "Product", EntityId = "101", UserId = "u1" }
            }
        };

        var count = await _service.TrackBatchActivitiesAsync(batch);

        Assert.Equal(4, count);
        Assert.Equal(4, await _dbContext.UserActivities.CountAsync());
    }

    [Fact]
    public async Task GetActivitiesAsync_WithFiltering_ReturnsPagedFilteredResults()
    {
        var activities = new List<UserActivity>
        {
            new() { Platform = "bazaar-azad", Action = "product_view", EntityType = "Product", EntityId = "1", UserId = "u1", CreatedTime = DateTime.UtcNow.AddMinutes(-10) },
            new() { Platform = "bazaar-azad", Action = "product_view", EntityType = "Product", EntityId = "2", UserId = "u2", CreatedTime = DateTime.UtcNow.AddMinutes(-5) },
            new() { Platform = "store-panel", Action = "order_status_change", EntityType = "Order", EntityId = "99", UserId = "admin", CreatedTime = DateTime.UtcNow }
        };

        await _dbContext.UserActivities.AddRangeAsync(activities);
        await _dbContext.SaveChangesAsync();

        var filter = new UserActivityFilterDto
        {
            Platform = "bazaar-azad",
            Action = "product_view",
            Page = 1,
            PageSize = 10
        };

        var result = await _service.GetActivitiesAsync(filter);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => Assert.Equal("bazaar-azad", item.Platform));
    }

    [Fact]
    public async Task GetActivitySummaryAsync_AggregatesBreakdownAndTopEntities()
    {
        var activities = new List<UserActivity>
        {
            new() { Platform = "bazaar-azad", Action = "product_view", EntityType = "Product", EntityId = "perfume-1", UserId = "u1", SessionId = "s1" },
            new() { Platform = "bazaar-azad", Action = "product_view", EntityType = "Product", EntityId = "perfume-1", UserId = "u2", SessionId = "s2" },
            new() { Platform = "bazaar-azad", Action = "product_view", EntityType = "Product", EntityId = "sport-shoes", UserId = "u1", SessionId = "s1" },
            new() { Platform = "bazaar-azad", Action = "search", EntityId = "creed aventus", UserId = "u1", SessionId = "s1" },
            new() { Platform = "store-panel", Action = "product_edit", EntityType = "Product", EntityId = "perfume-1", UserId = "merchant-1", SessionId = "s3" }
        };

        await _dbContext.UserActivities.AddRangeAsync(activities);
        await _dbContext.SaveChangesAsync();

        var summary = await _service.GetActivitySummaryAsync();

        Assert.NotNull(summary);
        Assert.Equal(5, summary.TotalEvents);
        Assert.Equal(3, summary.UniqueUsers);
        Assert.Equal(3, summary.UniqueSessions);
        Assert.True(summary.ActionBreakdown.ContainsKey("product_view"));
        Assert.Equal(3, summary.ActionBreakdown["product_view"]);
        Assert.NotEmpty(summary.TopViewedProducts);
        Assert.Equal("perfume-1", summary.TopViewedProducts[0].EntityId);
        Assert.Equal(2, summary.TopViewedProducts[0].Count);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
