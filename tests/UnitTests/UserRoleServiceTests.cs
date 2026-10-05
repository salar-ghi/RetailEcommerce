using System.Diagnostics;
using Application.DTOs;
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

public class UserRoleServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserRoleService _userRoleService;
    private readonly ITestOutputHelper _output;

    public UserRoleServiceTests(ITestOutputHelper output)
    {
        _output = output;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        _unitOfWork = new UnitOfWork(_dbContext, mockCache.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        _mapper = config.CreateMapper();

        _userRoleService = new UserRoleService(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task GetUserRoleByIdAsync_ExistingRole_ReturnsUserRoleDto()
    {
        // Arrange
        var userRole = new UserRole { Id = 1, UserId = "user-1", RoleId = 10 };
        await _dbContext.UserRoles.AddAsync(userRole);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _userRoleService.GetUserRoleByIdAsync("user-1", 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user-1", result.UserId);
        Assert.Equal(10, result.RoleId);
    }

    [Fact]
    public async Task GetUserRoleByIdAsync_NonExistingRole_ThrowsKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userRoleService.GetUserRoleByIdAsync("user-nonexistent", 999));
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ExistingRole_UpdatesSuccessfully()
    {
        // Arrange
        var userRole = new UserRole { Id = 1, UserId = "user-1", RoleId = 10 };
        await _dbContext.UserRoles.AddAsync(userRole);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var dtoToUpdate = new UserRoleDto { UserId = "user-1", RoleId = 10 };

        // Act
        await _userRoleService.UpdateUserRoleAsync(dtoToUpdate);

        // Assert
        var updated = await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == "user-1" && ur.RoleId == 10);
        Assert.NotNull(updated);
        Assert.Equal("user-1", updated.UserId);
        Assert.Equal(10, updated.RoleId);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_NonExistingRole_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dtoToUpdate = new UserRoleDto { UserId = "user-nonexistent", RoleId = 999 };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userRoleService.UpdateUserRoleAsync(dtoToUpdate));
    }

    [Fact]
    public async Task UpdateUserRoleAsync_PerformanceBenchmark()
    {
        // Arrange - Seed large number of user roles
        int count = 5000;
        var userRoles = new List<UserRole>(count);
        for (int i = 1; i <= count; i++)
        {
            userRoles.Add(new UserRole { Id = i, UserId = $"user-{i}", RoleId = i % 5 + 1 });
        }
        await _dbContext.UserRoles.AddRangeAsync(userRoles);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var targetDto = new UserRoleDto { UserId = $"user-{count / 2}", RoleId = (count / 2) % 5 + 1 };

        // Measure targeted lookup & update performance
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            await _userRoleService.UpdateUserRoleAsync(targetDto);
        }
        sw.Stop();

        _output.WriteLine($"100 Targeted UpdateUserRoleAsync calls on table of {count} items took: {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalMicroseconds} us)");

        // Assert update took place
        var updated = await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == targetDto.UserId && ur.RoleId == targetDto.RoleId);
        Assert.NotNull(updated);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
