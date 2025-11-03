using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
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
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
        }

        public async Task ConsumeAsync(string topic, Func<string, Task> handler, CancellationToken ct = default)
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
            consumer.Subscribe(topic);

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(ct);
                        if (result?.Message?.Value != null)
                            await handler(result.Message.Value);
                    }
                    catch (ConsumeException ex)
                    {
                        Console.Error.WriteLine($"[KafkaConsumer] Erro ao consumir tópico '{topic}': {ex.Error.Reason}");
                        await Task.Delay(2000, ct); // retry leve
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
