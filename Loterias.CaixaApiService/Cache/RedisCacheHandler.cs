using StackExchange.Redis;

namespace Loterias.CaixaApiService.Cache
{
    public class RedisCacheHandler
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly int _ttl;

        public RedisCacheHandler(IConfiguration config, IConnectionMultiplexer connection)
        {
            _connection = connection;
            _ttl = int.Parse(config["Redis:DefaultTTLMinutes"] ?? "360");
        }

        public async Task<string?> GetAsync(string key)
        {
            var db = _connection.GetDatabase();
            var value = await db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task SetAsync(string key, string value)
        {
            var db = _connection.GetDatabase();
            await db.StringSetAsync(key, value, TimeSpan.FromMinutes(_ttl));
        }
    }
}
