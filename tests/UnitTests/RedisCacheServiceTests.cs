using System.Net.Sockets;
using System.Text.Json;
using Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace UnitTests;

public class RedisCacheServiceTests
{
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly Mock<ILogger<RedisCacheService>> _mockLogger;
    private readonly RedisCacheService _cacheService;

    public RedisCacheServiceTests()
    {
        _mockCache = new Mock<IDistributedCache>();
        _mockLogger = new Mock<ILogger<RedisCacheService>>();
        _cacheService = new RedisCacheService(_mockCache.Object, _mockLogger.Object);
    }

    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task GetCachedDataAsync_NullOrWhiteSpaceKey_ReturnsDefaultWithoutCacheCall()
    {
        var resultNull = await _cacheService.GetCachedDataAsync<TestData>(null!);
        var resultEmpty = await _cacheService.GetCachedDataAsync<TestData>("   ");

        Assert.Null(resultNull);
        Assert.Null(resultEmpty);
        _mockCache.Verify(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetCachedDataAsync_ValidKeyAndData_ReturnsDeserializedObject()
    {
        var expected = new TestData { Id = 1, Name = "Item1" };
        var json = JsonSerializer.Serialize(expected);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        _mockCache.Setup(c => c.GetAsync("key1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        var result = await _cacheService.GetCachedDataAsync<TestData>("key1");

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Item1", result.Name);
    }

    [Fact]
    public async Task GetCachedDataAsync_JsonException_ReturnsDefault()
    {
        var invalidJsonBytes = System.Text.Encoding.UTF8.GetBytes("{ invalid json }");
        _mockCache.Setup(c => c.GetAsync("key1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidJsonBytes);

        var result = await _cacheService.GetCachedDataAsync<TestData>("key1");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCachedDataAsync_RedisException_ReturnsDefault()
    {
        _mockCache.Setup(c => c.GetAsync("key1", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Connection failed"));

        var result = await _cacheService.GetCachedDataAsync<TestData>("key1");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCachedDataAsync_TimeoutException_ReturnsDefault()
    {
        _mockCache.Setup(c => c.GetAsync("key1", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("Operation timed out"));

        var result = await _cacheService.GetCachedDataAsync<TestData>("key1");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCachedDataAsync_UnhandledException_PropagatesException()
    {
        _mockCache.Setup(c => c.GetAsync("key1", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Fatal error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _cacheService.GetCachedDataAsync<TestData>("key1"));
    }

    [Fact]
    public async Task SetCachedDataAsync_NullOrEmptyKey_ReturnsWithoutSettingCache()
    {
        var data = new TestData { Id = 1, Name = "Test" };

        await _cacheService.SetCachedDataAsync("", data, TimeSpan.FromMinutes(5));

        _mockCache.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetCachedDataAsync_InvalidExpiration_ReturnsWithoutSettingCache()
    {
        var data = new TestData { Id = 1, Name = "Test" };

        await _cacheService.SetCachedDataAsync("key1", data, TimeSpan.Zero);

        _mockCache.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetCachedDataAsync_ValidInputs_CallsSetAsync()
    {
        var data = new TestData { Id = 1, Name = "Test" };

        await _cacheService.SetCachedDataAsync("key1", data, TimeSpan.FromMinutes(5));

        _mockCache.Verify(c => c.SetAsync("key1", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetCachedDataAsync_RedisException_CatchesGracefully()
    {
        var data = new TestData { Id = 1, Name = "Test" };
        _mockCache.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RedisTimeoutException("Timeout", CommandStatus.WaitingToBeSent));

        var exception = await Record.ExceptionAsync(() => _cacheService.SetCachedDataAsync("key1", data, TimeSpan.FromMinutes(5)));

        Assert.Null(exception);
    }

    [Fact]
    public async Task SetCachedDataAsync_UnhandledException_PropagatesException()
    {
        var data = new TestData { Id = 1, Name = "Test" };
        _mockCache.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Fatal error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _cacheService.SetCachedDataAsync("key1", data, TimeSpan.FromMinutes(5)));
    }

    [Fact]
    public async Task RemoveCachedDataAsync_NullOrEmptyKey_ReturnsWithoutRemovingCache()
    {
        await _cacheService.RemoveCachedDataAsync("   ");

        _mockCache.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveCachedDataAsync_ValidKey_CallsRemoveAsync()
    {
        await _cacheService.RemoveCachedDataAsync("key1");

        _mockCache.Verify(c => c.RemoveAsync("key1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveCachedDataAsync_RedisException_CatchesGracefully()
    {
        _mockCache.Setup(c => c.RemoveAsync("key1", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RedisServerException("ERR"));

        var exception = await Record.ExceptionAsync(() => _cacheService.RemoveCachedDataAsync("key1"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task RemoveCachedDataAsync_UnhandledException_PropagatesException()
    {
        _mockCache.Setup(c => c.RemoveAsync("key1", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Fatal error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _cacheService.RemoveCachedDataAsync("key1"));
    }
}
