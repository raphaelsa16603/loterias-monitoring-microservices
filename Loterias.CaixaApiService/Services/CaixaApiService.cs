using Loterias.CaixaApiService.Cache;
using Loterias.CaixaApiService.Models;
using Loterias.CaixaApiService.Services.Interfaces;
using Loterias.CaixaClientLib;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Loterias.Logging.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prometheus;

namespace Loterias.CaixaApiService.Services
{
    public class CaixaApiService : ICaixaApiService
    {
        private readonly ICaixaApiClient _client;
        private readonly RedisCacheHandler _cache;
        private readonly IConfiguration _config;
        private readonly IStructuredLogger _logger;


        // Métricas (opcional, mas útil)
        private static readonly Counter CacheHits =
            Metrics.CreateCounter("caixaapi_cache_hits_total", "Cache hits do CaixaApiService", new CounterConfiguration
            {
                LabelNames = new[] { "endpoint", "tipo" }
            });

        private static readonly Counter CacheMisses =
            Metrics.CreateCounter("caixaapi_cache_misses_total", "Cache misses do CaixaApiService", new CounterConfiguration
            {
                LabelNames = new[] { "endpoint", "tipo" }
            });

        public CaixaApiService(ICaixaApiClient client, 
            RedisCacheHandler cache,  
            IConfiguration config,
            IStructuredLogger logger)
        {
            _client = client;
            _cache = cache;
            _config = config;
            _logger = logger;
        }

        // Adicione um método de conversão de CaixaResponse para CaixaResponseDto
        private CaixaResponseDto MapToDto(CaixaResponse response)
        {
            return new CaixaResponseDto
            {
                TipoLoteria = response.TipoJogo,
                Concurso = response.NumeroConcurso,
                DataSorteio = response.DataApuracao,
                LocalSorteio = response.LocalSorteio,
                NomeMunicipioUFSorteio = response.NomeMunicipioUFSorteio,
                ListaDezenas = response.ListaDezenas,
                DezenasEmOrdem = response.DezenasSorteadasOrdemSorteio,
                DezenasSegundoSorteio = response.ListaDezenasSegundoSorteio,
                TrevosSorteados = response.TrevosSorteados,
                NomeTimeCoracaoMesSorte = response.NomeTimeCoracaoMesSorte,
                ArrecadacaoTotal = response.ValorArrecadado,
                Acumulado = response.Acumulado,
                ValorAcumuladoProxConcurso = response.ValorAcumuladoProximoConcurso,
                ValorAcumuladoConcursoEspecial = response.ValorAcumuladoConcursoEspecial,
                ValorEstimadoProximoConcurso = response.ValorEstimadoProximoConcurso,
                ValorSaldoReservaGarantidora = response.ValorSaldoReservaGarantidora,
                ValorTotalPremioFaixaUm = response.ValorTotalPremioFaixaUm,
                Premiacoes = response.Premiacao?.Select(p => new PremiacaoDto
                {
                    Faixa = p.Faixa,
                    Descricao = p.DescricaoFaixa,
                    Ganhadores = p.NumeroDeGanhadores,
                    ValorPremio = p.ValorPremio
                }).ToList(),
                MunicipiosGanhadores = response.ListaMunicipioUFGanhadores?.Select(m => new MunicipioUFGanhadorDto
                {
                    Municipio = m.Municipio,
                    UF = m.UF,
                    Ganhadores = m.Ganhadores,
                    Posicao = m.Posicao,
                    Serie = m.Serie,
                    NomeFantasiaUL = m.NomeFantasiaUL
                }).ToList(),
                ResultadosEsportivos = response.ListaResultadoEquipeEsportiva?.Select(r => new ResultadoEquipeEsportivaDto
                {
                    DiaSemana = r.DiaSemana,
                    DataJogo = r.DataJogo,
                    Campeonato = r.Campeonato,
                    EquipeUm = r.EquipeUm,
                    EquipeDois = r.EquipeDois,
                    GolsEquipeUm = r.GolsEquipeUm,
                    GolsEquipeDois = r.GolsEquipeDois
                }).ToList(),
                DataProximoConcurso = response.DataProximoConcurso,
                NumeroConcursoProximo = response.NumeroConcursoProximo,
                Observacao = response.Observacao
            };
        }

        public async Task<CaixaResponseDto?> ObterUltimoAsync(string tipo)
        {
            string cacheKey = $"caixa:{tipo}:ultimo";

            try
            {
                var cached = await _cache.GetAsync(cacheKey);
                if (!string.IsNullOrEmpty(cached))
                {
                    _logger.Info($"Cache hit for {cacheKey}/{tipo}");
                    CacheHits.WithLabels(cacheKey, tipo).Inc();
                    return JsonConvert.DeserializeObject<CaixaResponseDto>(cached);
                }

                var result = await _client.ObterUltimoResultadoAsync(tipo);
                CacheMisses.WithLabels(cacheKey, tipo).Inc();

                if (result != null)
                {
                    var dto = MapToDto(result);
                    await _cache.SetAsync(cacheKey, JsonConvert.SerializeObject(dto));
                    _logger.Info($"Cache updated {cacheKey}/{tipo}");
                    return dto;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter último resultado para {tipo}", ex);
            }

            return null;
        }

        public async Task<CaixaResponseDto?> ObterPorConcursoAsync(string tipo, int concurso)
        {
            string cacheKey = $"caixa:{tipo}:{concurso}";
            try
            {
                var cached = await _cache.GetAsync(cacheKey);
                if (!string.IsNullOrEmpty(cached))
                {
                    _logger.Info($"Cache hit for {concurso}/{tipo}");
                    CacheHits.WithLabels(cacheKey, tipo).Inc();
                    return JsonConvert.DeserializeObject<CaixaResponseDto>(cached);
                }

                var result = await _client.ObterResultadoPorConcursoAsync(tipo, concurso);
                CacheMisses.WithLabels(cacheKey, tipo).Inc();
                if (result != null)
                {
                    var dto = MapToDto(result);
                    await _cache.SetAsync(cacheKey, JsonConvert.SerializeObject(dto));
                    _logger.Info($"Cache updated {cacheKey}/{tipo}");
                    return dto;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao obter resultado para concurso {concurso} e tipo {tipo}", ex);
            }

            return null;
        }

        public async Task<CaixaResponseDto?> AtualizarCacheAsync(string tipo)
        {
            _logger.Info($"Atualizando cache manualmente : {tipo}");
            try
            {
                var result = await _client.ObterUltimoResultadoAsync(tipo);
                if (result != null)
                {
                    var dto = MapToDto(result);
                    await _cache.SetAsync($"caixa:{tipo}:ultimo", JsonConvert.SerializeObject(dto));
                    return dto;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao atualizar cache para {tipo}", ex);
            }

            return null;
        }


    }
}

