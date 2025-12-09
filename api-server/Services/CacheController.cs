using StackExchange.Redis;
using System.Text.Json;

namespace api_server.Services;

public class CacheController<T> : ICachingSystem<T>
{
    private readonly IDatabase _redisDatabase;
    
    public CacheController(ConnectionMultiplexer redisMux)
    {
        _redisDatabase = redisMux.GetDatabase();
    }


    public async Task<T> CreateValue(string key, T value, double ttlMilliseconds)
    {
        var serializedValue =  JsonSerializer.Serialize(value);
        var redisResult = await _redisDatabase.StringSetAsync(key, serializedValue, expiry: TimeSpan.FromMilliseconds(ttlMilliseconds));
        if(!redisResult)
            throw new Exception($"Failed to register valid of key {key} in Redis!");
        return value;
    }

    public async Task<T?> GetValue(string key)
    {
        var redisResult = await _redisDatabase.StringGetAsync(key);
        if (!redisResult.HasValue)
            return default;
        return JsonSerializer.Deserialize<T>(redisResult);
    }

    public async Task<T?> UpdateValue(string key, T newValue)
    {
        var redisResult = await _redisDatabase.StringSetAsync(key, JsonSerializer.Serialize(newValue), keepTtl: true);
        if(!redisResult)
            throw new Exception($"Failed to update value of key {key} in Redis!");
        return newValue;
    }

    public async Task<T?> DeleteValue(string key)
    {
        var redisResult = await _redisDatabase.StringGetDeleteAsync(key);
        if (!redisResult.HasValue)
            throw new Exception($"Failed to delete value of key {key} in Redis!");
        return JsonSerializer.Deserialize<T>(redisResult);
    }
}