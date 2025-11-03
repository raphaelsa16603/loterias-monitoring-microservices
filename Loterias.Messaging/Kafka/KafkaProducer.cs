using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Loterias.Messaging.Interfaces;

namespace Loterias.Messaging.Kafka
{
    public class KafkaProducer : IMessageProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer(KafkaSettings settings)
        {
            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = settings?.BootstrapServers ?? "localhost:9092",
                    Acks = Acks.All,
                    MessageTimeoutMs = 5000
                };

                _producer = new ProducerBuilder<string, string>(config).Build();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Falha ao inicializar KafkaProducer: {ex.Message}", ex);
            }
        }

        public async Task PublishAsync<T>(string topic, T message, CancellationToken ct = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var kafkaMessage = new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = json
                };

                await _producer.ProduceAsync(topic, kafkaMessage, ct);
            }
            catch (ProduceException<string, string> ex)
            {
                // Erros típicos de rede, timeout ou broker
                Console.Error.WriteLine($"[KafkaProducer] Erro ao publicar no tópico '{topic}': {ex.Error.Reason}");
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[KafkaProducer] Erro inesperado: {ex.Message}");
                throw;
            }
        }

        public void Dispose() => _producer?.Dispose();
    }
}
