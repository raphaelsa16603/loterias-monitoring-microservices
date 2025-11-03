using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Threading;
using Loterias.Messaging.Interfaces;

namespace Loterias.Messaging.Kafka
{
    public class KafkaConsumer : IMessageConsumer
    {
        private readonly ConsumerConfig _config;

        public KafkaConsumer(KafkaSettings settings)
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public async Task ConsumeAsync(string topic, Func<string, Task> handler, CancellationToken ct = default)
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
            consumer.Subscribe(topic);
            while (!ct.IsCancellationRequested)
            {
                var result = consumer.Consume(ct);
                await handler(result.Message.Value);
            }
        }
    }
}

