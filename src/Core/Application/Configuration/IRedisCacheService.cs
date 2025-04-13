namespace Application.Configuration;

public interface IRedisCacheService
{
    Task<T> GetCachedDataAsync<T>(string key);
    Task SetCachedDataAsync<T>(string key, T data, TimeSpan expiration);
    Task RemoveCachedDataAsync(string Key);
}
