namespace Infrastructure.Caching;

public class RedisCacheService : IRedisCacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetCachedDataAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("Cache read was skipped because the provided key was empty.");
            return default;
        }

        try
        {
            var data = await _cache.GetStringAsync(key);
            return string.IsNullOrWhiteSpace(data) ? default : JsonSerializer.Deserialize<T>(data, JsonOptions);
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(exception,
                "Cached value for key '{CacheKey}' could not be deserialized. The cache entry will be ignored.",
                key);
            return default;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception,
                "Cache read failed for key '{CacheKey}'. The application will continue without cached data.",
                key);
            return default;
        }
    }

    public async Task SetCachedDataAsync<T>(string key, T data, TimeSpan expiration)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("Cache write was skipped because the provided key was empty.");
            return;
        }

        if (expiration <= TimeSpan.Zero)
        {
            _logger.LogWarning("Cache write was skipped for key '{CacheKey}' because the expiration must be positive.", key);
            return;
        }

        try
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(data, JsonOptions), options);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception,
                "Cache write failed for key '{CacheKey}'. The application will continue without updating the cache.",
                key);
        }
    }

    public async Task RemoveCachedDataAsync(string Key)
    {
        if (string.IsNullOrWhiteSpace(Key))
        {
            _logger.LogWarning("Cache removal was skipped because the provided key was empty.");
            return;
        }

        try
        {
            await _cache.RemoveAsync(Key);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception,
                "Cache removal failed for key '{CacheKey}'. The application will continue.",
                Key);
        }
    }
}
