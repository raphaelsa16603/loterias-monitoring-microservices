using System.Text.Json;
using Loterias.WriteApiService.Config;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Loterias.WriteApiService.Cache
{
    public class RedisCacheHandler
    {
        private readonly IConnectionMultiplexer _mux;
        private readonly IDatabase _db;
        private readonly TimeSpan _defaultTtl;

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public RedisCacheHandler(IConnectionMultiplexer mux, IOptions<RedisSettings> options)
        {
            _mux = mux;
            _db = _mux.GetDatabase();
            _defaultTtl = TimeSpan.FromMinutes(options.Value.DefaultTTLMinutes <= 0 ? 60 : options.Value.DefaultTTLMinutes);
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? ttl = null)
        {
            var json = JsonSerializer.Serialize(value, JsonOpts);
            return await _db.StringSetAsync(key, json, ttl ?? _defaultTtl);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var val = await _db.StringGetAsync(key);
            if (val.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(val!, JsonOpts);
        }

        public async Task<bool> RemoveAsync(string key) =>
            await _db.KeyDeleteAsync(key);

        public async Task<bool> ExistsAsync(string key) =>
            await _db.KeyExistsAsync(key);
    }
}
