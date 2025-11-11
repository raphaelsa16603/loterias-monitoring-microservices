namespace Loterias.JobConsumerService.Config
{
    public sealed class JobConsumerSettings
    {
        public string BootstrapServers { get; set; } = "localhost:9092";
        public string GroupId { get; set; } = "loterias-consumer";
        public List<string>? Topics { get; set; }
    }
}
