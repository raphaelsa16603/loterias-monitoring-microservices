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

                    // 🔹 Desserializa o JSON para objeto dinâmico
                    var doc = System.Text.Json.JsonDocument.Parse(message);
                    using var output = new System.IO.MemoryStream();
                    using (var writer = new System.Text.Json.Utf8JsonWriter(output))
                    {
                        writer.WriteStartObject();

                        // 🔹 Copia todas as propriedades, exceto "Id"
                        foreach (var prop in doc.RootElement.EnumerateObject())
                        {
                            if (prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                                continue;

                            // 🔹 Se for o array Premiacoes, remove também os Ids internos
                            if (prop.Name.Equals("Premiacoes", StringComparison.OrdinalIgnoreCase) && prop.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                            {
                                writer.WritePropertyName(prop.Name);
                                writer.WriteStartArray();

                                foreach (var prem in prop.Value.EnumerateArray())
                                {
                                    writer.WriteStartObject();
                                    foreach (var sub in prem.EnumerateObject())
                                    {
                                        if (sub.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                                            continue;
                                        sub.WriteTo(writer);
                                    }
                                    writer.WriteEndObject();
                                }

                                writer.WriteEndArray();
                            }
                            else
                            {
                                prop.WriteTo(writer);
                            }
                        }

                        writer.WriteEndObject();
                    }

                    // 🔹 Converte o JSON final sem IDs
                    var jsonSemId = System.Text.Encoding.UTF8.GetString(output.ToArray());

                    // 🔹 Envia para a Write API
                    var content = new StringContent(jsonSemId, System.Text.Encoding.UTF8, "application/json");
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