using Loterias.CollectorDailyService.Services.Interfaces;
using Loterias.Messaging.Interfaces;
using Loterias.Shared.Models;
using Loterias.Logging.Common.Interfaces;
using Loterias.CaixaApiService.Models; // <── para acessar o CaixaResponseDto
using System.Net.Http.Json;

namespace Loterias.CollectorDailyService.Services
{
    public class CollectorDailyService : ICollectorDailyService
    {
        private readonly HttpClient _httpClientCaixa;
        private readonly HttpClient _httpClientQuery;
        private readonly IMessageProducer _producer;
        private readonly IStructuredLogger _logger;

        private readonly List<string> _tiposJogo = new()
        {
            "megasena",
            "lotofacil",
            "quina",
            "lotomania",
            "timemania",
            "diadesorte",
            "duplasena",
            "supersete",
            "maismilionaria",
            "federal",
            "loteca"
        };

        public CollectorDailyService(
            IHttpClientFactory httpClientFactory,
            IMessageProducer producer,
            IStructuredLogger logger)
        {
            _httpClientCaixa = httpClientFactory.CreateClient("CaixaApi");
            _httpClientQuery = httpClientFactory.CreateClient("QueryApi");
            _producer = producer;
            _logger = logger;
        }

        public async Task ExecutarAsync(CancellationToken cancellationToken = default)
        {
            _logger.Info("Iniciando coleta diária das loterias...");

            foreach (var tipo in _tiposJogo)
            {
                try
                {
                    // 🔹 Passo 1 — Obtém dados crus da Caixa
                    var dto = await _httpClientCaixa.GetFromJsonAsync<CaixaResponseDto>($"api/v1/caixa/{tipo}/ultimo", cancellationToken);
                    if (dto == null || dto.Concurso == null)
                    {
                        _logger.Warn($"[{tipo}] Nenhum sorteio retornado pela API da Caixa.");
                        continue;
                    }

                    // 🔹 Passo 2 — Verifica se o concurso já está gravado
                    var existente = await _httpClientQuery.GetAsync($"api/v1/loterias/{tipo}/{dto.Concurso}", cancellationToken);
                    if (existente.IsSuccessStatusCode)
                    {
                        _logger.Info($"[{tipo}] Concurso {dto.Concurso} já existe — ignorando.");
                        continue;
                    }

                    // 🔹 Passo 3 — Mapeia para o modelo de domínio (Sorteio)
                    var sorteio = new Sorteio
                    {
                        TipoLoteria = dto.TipoLoteria ?? tipo.ToUpper(),
                        Concurso = dto.Concurso ?? 0,
                        DataSorteio = dto.DataSorteio ?? DateTime.MinValue,
                        LocalSorteio = dto.LocalSorteio ?? string.Empty,
                        NumerosSorteados = dto.ListaDezenas ?? new List<string>(),
                        DezenasEmOrdem = dto.DezenasEmOrdem ?? new List<string>(),
                        DezenasSegundoSorteio = dto.DezenasSegundoSorteio ?? new List<string>(),
                        TrevosSorteados = dto.TrevosSorteados ?? new List<string>(),
                        NomeTimeCoracaoMesSorte = dto.NomeTimeCoracaoMesSorte ?? string.Empty,
                        ArrecadacaoTotal = dto.ArrecadacaoTotal ?? 0,
                        Acumulado = dto.Acumulado ?? false,
                        ValorAcumuladoProxConcurso = dto.ValorAcumuladoProxConcurso ?? 0,
                        ValorEstimadoProximoConcurso = dto.ValorEstimadoProximoConcurso ?? 0,
                        Premiacoes = dto.Premiacoes?.Select(p => new Premiacao
                        {
                            TipoLoteria = dto.TipoLoteria ?? tipo.ToUpper(),
                            Concurso = dto.Concurso ?? 0,
                            Faixa = p.Faixa ?? 0,
                            Descricao = p.Descricao ?? "",
                            Ganhadores = p.Ganhadores ?? 0,
                            ValorPremio = p.ValorPremio ?? 0,
                            DataSorteio = dto.DataSorteio ?? DateTime.MinValue
                        }).ToList() ?? new List<Premiacao>(),
                        Observacao = dto.Observacao ?? string.Empty,
                        JsonCompleto = System.Text.Json.JsonSerializer.Serialize(dto)
                    };

                    // 🔹 Passo 4 — Publica no Kafka
                    await _producer.PublishAsync($"loterias.{tipo}", sorteio);
                    _logger.Info($"[{tipo}] Publicado novo sorteio {sorteio.Concurso} no tópico Kafka.");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Erro ao processar tipo {tipo}: {ex.Message}", ex);
                }
            }

            _logger.Info("Coleta diária concluída.");
        }
    }
}
