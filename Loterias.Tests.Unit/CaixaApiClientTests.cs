using Loterias.CaixaClientLib.Config;
using Loterias.CaixaClientLib.Enums;
using Loterias.CaixaClientLib.Exceptions;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Loterias.CaixaClientLib.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Loterias.Tests.Unit
{
    public class CaixaApiClientTests
    {
        private CaixaApiClient CreateClient(HttpMessageHandler handler, CaixaSettings? settings = null)
        {
            var httpClient = new HttpClient(handler);
            var options = Options.Create(settings ?? new CaixaSettings
            {
                BaseUrl = "https://servicebus2.caixa.gov.br/portaldeloterias/api/",
                TimeoutSeconds = 5,
                RetryCount = 2,
                EnableLogging = false
            });

            var loggerMock = new Mock<ILogger<CaixaApiClient>>();
            return new CaixaApiClient(httpClient, options, loggerMock.Object);
        }

        [Theory]
        [InlineData("megasena")]
        [InlineData("quina")]
        [InlineData("lotofacil")]
        [InlineData("lotomania")]
        [InlineData("timemania")]
        [InlineData("duplasena")]
        [InlineData("federal")]
        [InlineData("loteca")]
        [InlineData("diadesorte")]
        [InlineData("supersete")]
        [InlineData("maismilionaria")]
        public async Task ObterUltimoResultadoAsync_DeveDesserializarTodosOsCampos(string tipoJogo)
        {
            // Arrange
            var fakeResponse = new CaixaResponse
            {
                TipoJogo = tipoJogo.ToUpper(),
                NumeroConcurso = 9999,
                DataApuracao = DateTime.Now,
                ListaDezenas = new List<string> { "01", "02", "03" },
                Premiacao = new List<PremiacaoCaixa>
                {
                    new PremiacaoCaixa { Faixa = 1, DescricaoFaixa = "6 acertos", NumeroDeGanhadores = 0, ValorPremio = 0.0m }
                },
                LocalSorteio = "ESPAÇO DA SORTE",
                NomeMunicipioUFSorteio = "SÃO PAULO, SP",
                ValorArrecadado = 12345678.90m,
                ValorAcumuladoProximoConcurso = 1000000.00m,
                Acumulado = true,
                DataProximoConcurso = DateTime.Now.AddDays(2),
                NumeroConcursoProximo = 10000,
                TrevosSorteados = tipoJogo == "maismilionaria" ? new List<string> { "1", "4" } : null,
                ListaDezenasSegundoSorteio = tipoJogo == "duplasena"
                    ? new List<string> { "05", "10", "15", "20", "25", "30" }
                    : null
            };

            var json = JsonSerializer.Serialize(fakeResponse);
            var handler = new MockHttpMessageHandler(json, HttpStatusCode.OK);
            var client = CreateClient(handler);

            // Act
            var result = await client.ObterUltimoResultadoAsync(tipoJogo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeResponse.TipoJogo, result!.TipoJogo);
            Assert.Equal(fakeResponse.NumeroConcurso, result.NumeroConcurso);
            Assert.Equal(fakeResponse.LocalSorteio, result.LocalSorteio);
            Assert.Equal(fakeResponse.NomeMunicipioUFSorteio, result.NomeMunicipioUFSorteio);
            Assert.NotEmpty(result.ListaDezenas);

            if (tipoJogo == "maismilionaria")
                Assert.NotNull(result.TrevosSorteados);

            if (tipoJogo == "duplasena")
                Assert.NotNull(result.ListaDezenasSegundoSorteio);
        }

        [Fact]
        public async Task GetAsync_DeveTentarNovamenteEmErro()
        {
            var handler = new MockHttpMessageHandler("", HttpStatusCode.InternalServerError);
            var client = CreateClient(handler, new CaixaSettings
            {
                BaseUrl = "https://servicebus2.caixa.gov.br/portaldeloterias/api/",
                TimeoutSeconds = 1,
                RetryCount = 2,
                EnableLogging = false
            });

            await Assert.ThrowsAsync<CaixaApiException>(async () =>
            {
                await client.ObterUltimoResultadoAsync("megasena");
            });
        }

        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _response;
            private readonly HttpStatusCode _statusCode;

            public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
            {
                _response = response;
                _statusCode = statusCode;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var msg = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_response)
                };
                return Task.FromResult(msg);
            }
        }
    }
}
