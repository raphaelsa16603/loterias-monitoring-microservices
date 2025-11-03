using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;
using Loterias.Messaging.Interfaces;

namespace Loterias.Messaging.Kafka
{
    public class KafkaProducer : IMessageProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer(KafkaSettings settings)
        {
            var config = new ProducerConfig { BootstrapServers = settings?.BootstrapServers ?? "localhost:9092" };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync<T>(string topic, T message, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = json }, ct);
        }

        public void Dispose() => _producer?.Dispose();
    }
}

