using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Loterias.Messaging.Interfaces;
using Loterias.Logging.Common.Interfaces;

namespace Loterias.Messaging.Kafka
{
    public class KafkaConsumer : IMessageConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IStructuredLogger _logger;

        public KafkaConsumer(KafkaSettings settings, IStructuredLogger logger)
        {
            _logger = logger;

            _config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _logger.Info($"KafkaConsumer configurado para GroupId={_config.GroupId} em {_config.BootstrapServers}");
        }

        public async Task ConsumeAsync(string topic, Func<string, Task> handler, CancellationToken ct = default)
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
            consumer.Subscribe(topic);

            try
            {
                _logger.Info($"Consumidor iniciado para o tópico '{topic}'");

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(ct);
                        if (result?.Message?.Value != null)
                        {
                            _logger.Info($"Mensagem recebida no tópico '{topic}'", new { Offset = result.Offset });
                            await handler(result.Message.Value);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.Warn($"Erro ao consumir tópico '{topic}': {ex.Error.Reason}");
                        await Task.Delay(2000, ct); // retry leve
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Erro inesperado ao processar mensagem do tópico '{topic}'", ex);
                    }
                }
            }
            finally
            {
                consumer.Close();
                _logger.Info($"Consumidor encerrado para o tópico '{topic}'");
            }
        }
    }
}
