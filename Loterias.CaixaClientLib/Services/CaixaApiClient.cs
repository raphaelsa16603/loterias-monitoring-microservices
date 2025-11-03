using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Loterias.CaixaClientLib.Config;
using Loterias.CaixaClientLib.Enums;
using Loterias.CaixaClientLib.Exceptions;

namespace Loterias.CaixaClientLib.Services
{
    public class CaixaApiClient : ICaixaApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CaixaSettings _settings;
        private readonly CaixaEndpointsProvider _endpoints;
        private readonly ILogger<CaixaApiClient> _logger;

        public CaixaApiClient(HttpClient httpClient, IOptions<CaixaSettings> settings, ILogger<CaixaApiClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _endpoints = new CaixaEndpointsProvider(_settings.BaseUrl);

            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
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
                    var response = await _httpClient.GetAsync(fullUrl ? url : $"{_settings.BaseUrl}{url}");
                    if (!response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        throw new CaixaApiException(response.StatusCode, content);
                    }

                    var data = await response.Content.ReadFromJsonAsync<CaixaResponse>();
                    if (_settings.EnableLogging)
                        _logger.LogInformation("✅ Sucesso: {Url} ({Tipo}) Concurso {Conc}", url, data?.TipoJogo, data?.NumeroConcurso);

                    return data;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Tentativa {Tentativa}/{Total} falhou para {Url}", attempt, _settings.RetryCount, url);
                    if (attempt == _settings.RetryCount)
                        throw;
                    await Task.Delay(1000 * attempt); // backoff incremental
                }
            }

            return null;
        }
    }
}

