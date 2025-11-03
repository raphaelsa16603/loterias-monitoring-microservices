using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Loterias.Messaging.Interfaces;
using Loterias.Logging.Common.Interfaces;

namespace Loterias.Messaging.Kafka
{
    public class KafkaProducer : IMessageProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly IStructuredLogger _logger;

        public KafkaProducer(KafkaSettings settings, IStructuredLogger logger)
        {
            _logger = logger;

            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = settings?.BootstrapServers ?? "localhost:9092",
                    Acks = Acks.All,
                    MessageTimeoutMs = 5000
                };

                _producer = new ProducerBuilder<string, string>(config).Build();
                _logger.Info($"KafkaProducer inicializado com sucesso em {config.BootstrapServers}");
            }
            catch (Exception ex)
            {
                _logger.Error("Falha ao inicializar KafkaProducer", ex);
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
                _logger.Info($"Mensagem publicada no tópico '{topic}'", new { topic, message });
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.Error($"Erro ao publicar no tópico '{topic}': {ex.Error.Reason}", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro inesperado ao publicar no tópico '{topic}'", ex);
                throw;
            }
        }

        public void Dispose()
        {
            _producer?.Dispose();
            _logger.Info("KafkaProducer encerrado.");
        }
    }
}
