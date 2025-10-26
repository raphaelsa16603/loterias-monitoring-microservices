using Loterias.Shared.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Loterias.RedisCacheService.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        var json = JsonConvert.SerializeObject(value);
        await _db.StringSetAsync(key, json, ttl);
    }
}
