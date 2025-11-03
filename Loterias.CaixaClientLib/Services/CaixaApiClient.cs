using System.Net.Http.Json;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Loterias.CaixaClientLib.Config;
using Loterias.CaixaClientLib.Enums;
using Loterias.CaixaClientLib.Exceptions;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Loterias.CaixaClientLib.Services.Internal; // compat helper

namespace Loterias.CaixaClientLib.Services
{
    public class CaixaApiClient : ICaixaApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CaixaSettings _settings;
        private readonly CaixaEndpointsProvider _endpoints;

        // Logger comum (fallback) + structured (principal)
        private readonly ILogger<CaixaApiClient>? _logger;
        private readonly object? _structuredLogger; // não tipar para evitar acoplamento

        public CaixaApiClient(
            HttpClient httpClient,
            IOptions<CaixaSettings> settings,
            // structuredLogger vem da sua Loterias.Logging.Common (qualquer tipo/iface)
            object? structuredLogger = null,
            ILogger<CaixaApiClient>? logger = null)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _structuredLogger = structuredLogger;
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
                        await LoggingCompat.SafeLogAsync(_structuredLogger, _logger,
                            level: "INFO",
                            message: $"Sucesso ao consultar {targetUrl} ({data?.TipoJogo}) Concurso {data?.NumeroConcurso}",
                            data: new
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
                    await LoggingCompat.SafeLogAsync(_structuredLogger, _logger,
                        level: "WARN",
                        message: $"Tentativa {attempt}/{_settings.RetryCount} falhou ao acessar {url}",
                        data: new { Url = url, Tentativa = attempt },
                        exception: ex);

                    if (attempt == _settings.RetryCount)
                    {
                        await LoggingCompat.SafeLogAsync(_structuredLogger, _logger,
                            level: "ERROR",
                            message: $"Falha definitiva após {attempt} tentativas",
                            exception: ex);

                        throw;
                    }

                    await Task.Delay(1000 * attempt); // backoff incremental
                }
            }

            return null;
        }
    }
}
