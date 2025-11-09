namespace Loterias.Messaging.Kafka
{
    public class KafkaSettings
    {
        // conexão
        public string BootstrapServers { get; set; } = "kafka:9092";
        public string SecurityProtocol { get; set; } = "PLAINTEXT";

        // consumer
        public string GroupId { get; set; } = "loterias-consumers";
        public bool EnableAutoCommit { get; set; } = true;
        public int AutoOffsetReset { get; set; } = 1; // earliest

        // conveniências de publish
        public string BaseTopicName { get; set; } = "loterias";
        public int RetryCount { get; set; } = 3;
        public int PublishTimeoutMs { get; set; } = 3000;
    }
}
