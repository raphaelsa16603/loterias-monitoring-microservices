using Loterias.CaixaApiService.Cache;
using Loterias.CaixaApiService.Models;
using Loterias.CaixaApiService.Services.Interfaces;
using Loterias.CaixaClientLib;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Loterias.CaixaApiService.Services
{
    public class CaixaApiService : ICaixaApiService
    {
        private readonly ICaixaApiClient _client;
        private readonly RedisCacheHandler _cache;
        private readonly IConfiguration _config;

        public CaixaApiService(ICaixaApiClient client, RedisCacheHandler cache,  IConfiguration config)
        {
            _client = client;
            _cache = cache;
            _config = config;
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

            var cached = await _cache.GetAsync(cacheKey);
            if (cached != null)
            {
                Console.WriteLine($"Cache hit for {cacheKey}/{tipo}");
                return JsonConvert.DeserializeObject<CaixaResponseDto>(cached);
            }

            var result = await _client.ObterUltimoResultadoAsync(tipo);
            if (result != null)
            {
                await _cache.SetAsync(cacheKey, JsonConvert.SerializeObject(result));
                Console.WriteLine($"Cache updated {cacheKey}/{tipo}");
                return MapToDto(result);
            }

            return null;
        }

        public async Task<CaixaResponseDto?> ObterPorConcursoAsync(string tipo, int concurso)
        {
            string cacheKey = $"caixa:{tipo}:{concurso}";
            var cached = await _cache.GetAsync(cacheKey);
            if (cached != null)
            {
                Console.WriteLine($"Cache hit for {concurso}/{tipo}");
                return JsonConvert.DeserializeObject<CaixaResponseDto>(cached);
            }

            var result = await _client.ObterResultadoPorConcursoAsync(tipo, concurso);
            if (result != null)
            {
                await _cache.SetAsync(cacheKey, JsonConvert.SerializeObject(result));
                return MapToDto(result);
            }

            return null;
        }

        public async Task<CaixaResponseDto?> AtualizarCacheAsync(string tipo)
        {
            Console.WriteLine($"Atualizando cache manualmente : {tipo}");
            var result = await _client.ObterUltimoResultadoAsync(tipo);
            if (result != null)
            {
                await _cache.SetAsync($"caixa:{tipo}:ultimo", JsonConvert.SerializeObject(result));
                return MapToDto(result);
            }

            return null;
        }
    }
}

