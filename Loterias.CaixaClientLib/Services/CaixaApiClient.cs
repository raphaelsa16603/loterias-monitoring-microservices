using Loterias.CaixaClientLib.Config;
using Loterias.CaixaClientLib.Enums;
using Loterias.CaixaClientLib.Exceptions;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Loterias.Logging.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
                        IOptions<CaixaSettings> options,
                        IStructuredLogger logger)
        {
            _settings = options.Value ?? new CaixaSettings
            {
                BaseUrl = "https://servicebus2.caixa.gov.br/portaldeloterias/api/",
                TimeoutSeconds = 15,
                RetryCount = 3,
                EnableLogging = true
            };

            // Normaliza a URL base
            var normalizedBase = _settings.BaseUrl.TrimEnd('/') + "/";

            _httpClient = httpClient;
            _logger = logger;
            _endpoints = new CaixaEndpointsProvider(normalizedBase);

            _httpClient.BaseAddress = new Uri(normalizedBase);
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (compatible; LoteriasBot/1.0; +https://github.com/raphaelsa16603)");
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("pt-BR"));
        }


        public async Task<CaixaResponse?> ObterUltimoResultadoAsync(string tipoLoteria)
            => await GetAsync($"{tipoLoteria}/");

        public async Task<CaixaResponse?> ObterResultadoPorConcursoAsync(string tipoLoteria, int concurso)
            => await GetAsync($"{tipoLoteria}/{concurso}");

        public async Task<CaixaResponse?> ObterUltimoResultadoAsync(TipoLoteriaCaixa tipo)
            => await GetAsync(_endpoints.GetUrlUltimoResultado(tipo), true);

        public async Task<CaixaResponse?> ObterResultadoPorConcursoAsync(TipoLoteriaCaixa tipo, int concurso)
            => await GetAsync(_endpoints.GetUrlPorConcurso(tipo, concurso), true);

        private async Task<CaixaResponse?> GetAsync(string path, bool fullUrl = false)
        {
            for (int attempt = 1; attempt <= _settings.RetryCount; attempt++)
            {
                try
                {
                    var response = fullUrl
                        ? await _httpClient.GetAsync(path)
                        : await _httpClient.GetAsync(path.TrimStart('/'));

                    if (!response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        throw new CaixaApiException(response.StatusCode, content);
                    }

                    var data = await response.Content.ReadFromJsonAsync<CaixaResponse>();

                    if (_settings.EnableLogging)
                        _logger.Info("✅ Consulta Caixa OK", new { Url = response.RequestMessage?.RequestUri?.ToString(), data?.TipoJogo });

                    return data;
                }
                catch (Exception ex)
                {
                    _logger.Warn($"Tentativa {attempt}/{_settings.RetryCount} falhou", new { path, ex.Message });

                    if (attempt == _settings.RetryCount)
                    {
                        _logger.Error("❌ Falha definitiva", ex, new { path });
                        throw;
                    }

                    await Task.Delay(1000 * attempt);
                }
            }
            return null;
        }




    }
}
