using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Loterias.CaixaClientLib.Config;
using Loterias.CaixaClientLib.Enums;
using Loterias.CaixaClientLib.Exceptions;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Loterias.Logging.Common.Interfaces;

namespace Loterias.CaixaClientLib.Services
{
    public class CaixaApiClient : ICaixaApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CaixaSettings _settings;
        private readonly CaixaEndpointsProvider _endpoints;
        private readonly IStructuredLogger _logger;

        public CaixaApiClient(
            HttpClient httpClient,
            IOptions<CaixaSettings> settings,
            IStructuredLogger logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _endpoints = new CaixaEndpointsProvider(_settings.BaseUrl);

            _settings = settings.Value ?? new CaixaSettings
            {
                BaseUrl = "https://servicebus2.caixa.gov.br/portaldeloterias/api/",
                TimeoutSeconds = 15,
                RetryCount = 3,
                EnableLogging = true
            };


            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);

            // A API da Caixa bloqueia requisições sem User-Agent (desde 2023).
            // Quando não há, ela devolve 400 Bad Request, mesmo que a URL esteja certa.

            _httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; LoteriasBot/1.0; +https://github.com/raphaelsa16603)");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");

            // Habilita descompressão automática
            _httpClient.DefaultRequestHeaders.TransferEncodingChunked = false;
            _httpClient.DefaultRequestVersion = new Version(2, 0);
            _httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;

            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");

            // Força cliente a descomprimir gzip
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("TE", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");

            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://loterias.caixa.gov.br");


        }

        public async Task<CaixaResponse?> ObterUltimoResultadoAsync(string tipoLoteria)
            => await GetAsync($"{tipoLoteria}/ultimo");

        public async Task<CaixaResponse?> ObterResultadoPorConcursoAsync(string tipoLoteria, int concurso)
            => await GetAsync($"{tipoLoteria}/{concurso}");

        public async Task<CaixaResponse?> ObterUltimoResultadoAsync(TipoLoteriaCaixa tipo)
            => await GetAsync(_endpoints.GetUrlUltimoResultado(tipo), true);

        public async Task<CaixaResponse?> ObterResultadoPorConcursoAsync(TipoLoteriaCaixa tipo, int concurso)
            => await GetAsync(_endpoints.GetUrlPorConcurso(tipo, concurso), true);

        private async Task<CaixaResponse?> GetAsync(string url, bool fullUrl = false)
        {
            for (int attempt = 1; attempt <= _settings.RetryCount; attempt++)
            {
                try
                {
                    var targetUrl = fullUrl ? url : $"{_settings.BaseUrl}{url}";
                    var response = await _httpClient.GetAsync(targetUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        throw new CaixaApiException(response.StatusCode, content);
                    }

                    var data = await response.Content.ReadFromJsonAsync<CaixaResponse>();

                    if (_settings.EnableLogging)
                    {
                        _logger.Info(
                            $"✅ Sucesso ao consultar {targetUrl}",
                            new
                            {
                                Tipo = data?.TipoJogo,
                                Concurso = data?.NumeroConcurso,
                                Url = targetUrl
                            });
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    _logger.Warn(
                        $"Tentativa {attempt}/{_settings.RetryCount} falhou ao acessar {url}",
                        new { Url = url, Tentativa = attempt, Erro = ex.Message });

                    if (attempt == _settings.RetryCount)
                    {
                        _logger.Error(
                            $"❌ Falha definitiva após {attempt} tentativas em {url}",
                            ex,
                            new { Url = url, Tentativa = attempt });
                        throw;
                    }

                    await Task.Delay(1000 * attempt); // backoff incremental
                }
            }

            return null;
        }
    }
}
