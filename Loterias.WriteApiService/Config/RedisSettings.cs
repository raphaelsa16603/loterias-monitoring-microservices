namespace Loterias.WriteApiService.Config
{
    public sealed class RedisSettings
    {
        public string Host { get; set; } = "redis";
        public int Port { get; set; } = 6379;
        /// <summary>TTL padrão (minutos) para chaves de cache.</summary>
        public int DefaultTTLMinutes { get; set; } = 360;
    }
}
