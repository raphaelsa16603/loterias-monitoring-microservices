using Confluent.Kafka;
using Loterias.Logging.Common.Interfaces;
using Loterias.JobConsumerService.Config;
using Microsoft.Extensions.Options;

namespace Loterias.JobConsumerService.Consumers
{
    /// <summary>
    /// Consumer dedicado aos tópicos de sorteios (ex: loterias.megasena, loterias.quina...).
    /// Faz leitura contínua e entrega mensagens para processamento.
    /// </summary>
    public class SorteiosConsumer : IDisposable
    {
        private readonly IStructuredLogger _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly JobConsumerSettings _settings;
        private bool _disposed;

        public SorteiosConsumer(
            IStructuredLogger logger,
            IOptions<JobConsumerSettings> options)
        {
            _logger = logger;
            _settings = options.Value;

            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                AllowAutoCreateTopics = true
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) =>
                    _logger.Warn("SorteiosConsumer - KafkaError", e.Reason))
                .SetStatisticsHandler((_, json) =>
                    _logger.Info($"SorteiosConsumer - KafkaStats {json}"))
                .Build();

            _logger.Info("SorteiosConsumer - KafkaInit" +
                $"Conectado ao Kafka ({_settings.BootstrapServers}) - Grupo: {_settings.GroupId}");

            _logger.Info($"BootstrapServers: {_settings.BootstrapServers ?? "null"}");
            _logger.Info($"GroupId: {_settings.GroupId ?? "null"}");
            _logger.Info($"Topics: {(_settings.Topics == null ? "null" : _settings.Topics.Count.ToString())}");

        }

        /// <summary>
        /// Inscreve o consumer nos tópicos configurados.
        /// </summary>
        public void SubscribeTopics()
        {
            if (_settings.Topics == null || _settings.Topics.Count == 0)
                throw new InvalidOperationException("Nenhum tópico Kafka configurado para consumo.");

            _consumer.Subscribe(_settings.Topics);
            _logger.Info("SorteiosConsumer - KafkaSubscribe" +
                $"Inscrito nos tópicos: {string.Join(", ", _settings.Topics)}");
        }

        /// <summary>
        /// Loop principal de consumo Kafka. Retorna mensagens como strings JSON.
        /// </summary>
        public async IAsyncEnumerable<(string Topic, string Message)> ConsumeAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<Ignore, string>? result = null;
                try
                {
                    result = _consumer.Consume(cancellationToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.Error($"SorteiosConsumer - ConsumeException {ex.Error.Reason}", ex);
                    await Task.Delay(2000, cancellationToken); // espera antes de tentar novamente
                }
                catch (OperationCanceledException)
                {
                    _logger.Warn("SorteiosConsumer - KafkaStop - Consumo cancelado pelo token.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error($"SorteiosConsumer - UnexpectedError - {ex.Message}", ex);
                    await Task.Delay(5000, cancellationToken);
                }

                if (result == null)
                    continue;

                _logger.Info("SorteiosConsumer - KafkaMessage" +
                    $"Mensagem recebida de {result.Topic} (offset {result.Offset})");

                yield return (result.Topic, result.Message.Value);

                _consumer.Commit(result); // marca como processado
            }

            _consumer.Close();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _consumer.Close();
                _consumer.Dispose();
                _disposed = true;
            }
        }
    }
}
