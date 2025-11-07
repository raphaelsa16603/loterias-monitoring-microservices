using Loterias.CaixaApiService.Cache;
using Loterias.CaixaApiService.Models;
using Loterias.CaixaApiService.Services.Interfaces;
using Loterias.CaixaClientLib;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Loterias.Logging.Common;
using Loterias.Logging.Common.Interfaces;
using Newtonsoft.Json;

namespace Loterias.CaixaApiService.Services
{
    public class CaixaApiService : ICaixaApiService
    {
        private readonly ICaixaApiClient _client;
        private readonly RedisCacheHandler _cache;
        private readonly IStructuredLogger _logger;
        private readonly IConfiguration _config;

        public CaixaApiService(ICaixaApiClient client, RedisCacheHandler cache, IStructuredLogger logger, IConfiguration config)
        {
            _client = client;
            _cache = cache;
            _logger = logger;
            _config = config;
        }

        // Adicione um método de conversão de CaixaResponse para CaixaResponseDto
        private CaixaResponseDto MapToDto(CaixaResponse response)
        {
            CaixaResponseDto retorno = new CaixaResponseDto();

            try
            {
                retorno = new CaixaResponseDto
                {
                    TipoLoteria = response.TipoJogo,
                    Concurso = response.NumeroConcurso,
                    DataSorteio = response.DataApuracao,
                    ListaDezenas = response.Dezenas,
                    Premiacoes = response.Premiacao?.ConvertAll(p => new PremiacaoDto
                    {
                        Faixa = p.Faixa,
                        Descricao = p.DescricaoFaixa,
                        Ganhadores = p.NumeroDeGanhadores,
                        ValorPremio = p.ValorPremio,
                    }),

                    ArrecadacaoTotal = response.ValorArrecadado,
                    ValorAcumuladoProxConcurso = response.ValorAcumuladoProximoConcurso,
                    LocalSorteio = response.LocalSorteio,
                    DezenasEmOrdem = response.Dezenas.Order().ToList(),
                    NomeMunicipioUFSorteio = response.NomeMunicipioUFSorteio,
                    DataProximoConcurso = response.DataProximoConcurso,
                    NumeroConcursoProximo = response.NumeroConcursoProximo,
                    Observacao = response.Observacao,
                    Acumulado = response.Acumulado,


                };
            }
            catch (Exception ex)
            {
                _logger.Error("Erro ao mapear CaixaResponse para CaixaResponseDto", ex, new { response });
                try
                {
                    retorno.TipoLoteria = response.TipoJogo;
                    retorno.Concurso = response.NumeroConcurso;
                    retorno.DataSorteio = response.DataApuracao;
                    retorno.ListaDezenas = response.Dezenas;
                    retorno.Premiacoes = response.Premiacao?.ConvertAll(p => new PremiacaoDto
                    {
                        Faixa = p.Faixa,
                        Descricao = p.DescricaoFaixa,
                        Ganhadores = p.NumeroDeGanhadores,
                        ValorPremio = p.ValorPremio,
                    });
                    retorno.ArrecadacaoTotal = response.ValorArrecadado;
                    retorno.ValorAcumuladoProxConcurso = response.ValorAcumuladoProximoConcurso;
                    retorno.LocalSorteio = response.LocalSorteio;
                    retorno.DezenasEmOrdem = response.Dezenas.Order().ToList();
                    retorno.NomeMunicipioUFSorteio = response.NomeMunicipioUFSorteio;
                    retorno.DataProximoConcurso = response.DataProximoConcurso;
                    retorno.NumeroConcursoProximo = response.NumeroConcursoProximo;
                    retorno.Observacao = response.Observacao;
                    retorno.Acumulado = response.Acumulado;

                }
                catch
                {
                    // Ignora falhas adicionais
                }
            }

            // Converte as dezenas para inteiros
            var retornoNumeros = new List<int>();
            foreach (var dezena in response.Dezenas)
            {
                try
                {
                    if (int.TryParse(dezena, out int numero))
                    {
                        retornoNumeros.Add(numero);
                    }
                }
                catch
                {
                    // Ignora valores inválidos
                }
            }

            retorno.NumerosDoSorteio = retornoNumeros.ToArray();

            return retorno;
        }

        public async Task<CaixaResponseDto?> ObterUltimoAsync(string tipo)
        {
            string cacheKey = $"caixa:{tipo}:ultimo";

            var cached = await _cache.GetAsync(cacheKey);
            if (cached != null)
            {
                _logger.Info("Cache hit", new { Tipo = tipo });
                return JsonConvert.DeserializeObject<CaixaResponseDto>(cached);
            }

            var result = await _client.ObterUltimoResultadoAsync(tipo);
            if (result != null)
            {
                var dto = MapToDto(result);
                await _cache.SetAsync(cacheKey, JsonConvert.SerializeObject(dto));
                _logger.Info("Cache updated", new { Tipo = tipo });
                return dto;
            }

            return null;
        }

        public async Task<CaixaResponseDto?> ObterPorConcursoAsync(string tipo, int concurso)
        {
            string cacheKey = $"caixa:{tipo}:{concurso}";
            var cached = await _cache.GetAsync(cacheKey);
            if (cached != null)
            {
                _logger.Info("Cache hit", new { Tipo = tipo, Concurso = concurso });
                return JsonConvert.DeserializeObject<CaixaResponseDto>(cached);
            }

            var result = await _client.ObterResultadoPorConcursoAsync(tipo, concurso);
            if (result != null)
            {
                var dto = MapToDto(result);
                await _cache.SetAsync(cacheKey, JsonConvert.SerializeObject(dto));
                return dto;
            }

            return null;
        }

        public async Task<CaixaResponseDto?> AtualizarCacheAsync(string tipo)
        {
            _logger.Info("Atualizando cache manualmente", new { Tipo = tipo });
            var result = await _client.ObterUltimoResultadoAsync(tipo);
            if (result != null)
            {
                var dto = MapToDto(result);
                await _cache.SetAsync($"caixa:{tipo}:ultimo", JsonConvert.SerializeObject(dto));
                return dto;
            }

            return null;
        }
    }
}

