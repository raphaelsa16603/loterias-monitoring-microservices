using Loterias.JobConsumerService.Config;
using Loterias.JobConsumerService.Consumers;
using Loterias.Logging.Common.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Net.Http;

namespace Loterias.JobConsumerService.Services
{
    /// <summary>
    /// Serviço responsável por consumir mensagens dos tópicos Kafka
    /// e processá-las, enviando para a WriteApiService.
    /// </summary>
    public class JobExecutorService
    {
        private readonly SorteiosConsumer _consumer;
        private readonly HttpClient _http;
        private readonly IStructuredLogger _logger;
        private readonly string _writeApiBaseUrl;

        public JobExecutorService(SorteiosConsumer consumer, IStructuredLogger logger,
                                  IOptions<HttpSettings> httpOpts, IHttpClientFactory httpFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _writeApiBaseUrl = httpOpts.Value.BaseUrl;
            _http = httpFactory.CreateClient();
        }

        public async Task StartAsync(CancellationToken token)
        {
            _consumer.SubscribeTopics();

            await foreach (var (topic, message) in _consumer.ConsumeAsync(token))
            {
                try
                {
                    _logger.Info($"JobExecutorService - MessageProcessing - Processando mensagem de {topic}");
                    // Transmite o JSON bruto da mensagem Kafka diretamente como corpo da requisição
                    var content = new StringContent(message, System.Text.Encoding.UTF8, "application/json");
                    var response = await _http.PostAsync($"{_writeApiBaseUrl}/api/v1/write/sorteios", content, token);


                    if (response.IsSuccessStatusCode)
                        _logger.Info($"JobExecutorService - PersistenciaSucesso - Mensagem salva via WriteApi ({topic})");
                    else
                        _logger.Warn($"JobExecutorService - PersistenciaFalha", await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    _logger.Error($"JobExecutorService - ErroProcessamento {ex.Message}", ex);
                }
            }
        }
    }

}