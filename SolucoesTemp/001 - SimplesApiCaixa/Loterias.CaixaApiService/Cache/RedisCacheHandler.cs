namespace Loterias.CaixaApiService.Cache
{
    public class RedisCacheHandler
    {
        private readonly int _ttl;

        public RedisCacheHandler(IConfiguration config)
        {
            // _connection = connection;
            // _ttl = int.Parse(config["Redis:DefaultTTLMinutes"] ?? "360");
        }

        public async Task<string?> GetAsync(string key)
        {
            // var db = _connection.GetDatabase();
            // var value = await db.StringGetAsync(key);
            return null; // value.HasValue ? value.ToString() : null;
        }

        public async Task SetAsync(string key, string value)
        {
            // var db = _connection.GetDatabase();
            // await db.StringSetAsync(key, value, TimeSpan.FromMinutes(_ttl));
        }
    }
}
