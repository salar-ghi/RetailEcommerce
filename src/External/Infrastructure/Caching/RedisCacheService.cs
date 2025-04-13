namespace Infrastructure.Caching;
public class RedisCacheService: IRedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }


    public async Task<T> GetCachedDataAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        return data == null ? default : JsonSerializer.Deserialize<T>(data);

        //var cachedData = await _cache.GetStringAsync(key);
        //if (cachedData != null)
        //{
        //    return JsonSerializer.Deserialize<T>(cachedData);
        //}
        //return default;
    }

    public async Task SetCachedDataAsync<T>(string key, T data, TimeSpan expiration)
    {
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
    }

    public async Task RemoveCachedDataAsync(string Key)
    {
        await _cache.RemoveAsync(Key);
    }


}