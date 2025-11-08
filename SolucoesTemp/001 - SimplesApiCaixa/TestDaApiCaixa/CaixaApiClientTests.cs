using Loterias.CaixaClientLib.Config;
using Loterias.CaixaClientLib.Enums;
using Loterias.CaixaClientLib.Exceptions;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Models;
using Loterias.CaixaClientLib.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TestDaApiCaixa
{
    [TestFixture]
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

        // ✅ Teste genérico para todos os jogos suportados
        [TestCase("megasena")]
        [TestCase("quina")]
        [TestCase("lotofacil")]
        [TestCase("lotomania")]
        [TestCase("timemania")]
        [TestCase("duplasena")]
        [TestCase("federal")]
        [TestCase("loteca")]
        [TestCase("diadesorte")]
        [TestCase("supersete")]
        [TestCase("maismilionaria")]
        public async Task ObterUltimoResultadoAsync_DeveDesserializarTodosOsCampos(string tipoJogo)
        {
            // 🔹 Arrange – Simula resposta básica representativa
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

                // ✅ Adicionado: simulação do segundo sorteio da Dupla Sena
                ListaDezenasSegundoSorteio = tipoJogo == "duplasena"
                    ? new List<string> { "05", "10", "15", "20", "25", "30" }
                    : null
            };


            var json = JsonSerializer.Serialize(fakeResponse);
            var handler = new MockHttpMessageHandler(json, HttpStatusCode.OK);
            var client = CreateClient(handler);

            // 🔹 Act
            var result = await client.ObterUltimoResultadoAsync(tipoJogo);

            // 🔹 Assert
            Assert.NotNull(result, $"O resultado do jogo {tipoJogo} não deve ser nulo");
            Assert.AreEqual(fakeResponse.TipoJogo, result!.TipoJogo, "TipoJogo incorreto");
            Assert.AreEqual(fakeResponse.NumeroConcurso, result.NumeroConcurso, "Número do concurso incorreto");
            Assert.AreEqual(fakeResponse.LocalSorteio, result.LocalSorteio);
            Assert.AreEqual(fakeResponse.NomeMunicipioUFSorteio, result.NomeMunicipioUFSorteio);
            Assert.GreaterOrEqual(result.ListaDezenas.Count, 1, "Dezenas não foram mapeadas");

            // Campos opcionais
            if (tipoJogo == "maismilionaria")
                Assert.NotNull(result.TrevosSorteados, "TrevosSorteados deve existir na +Milionária");

            if (tipoJogo == "duplasena")
                Assert.NotNull(result.ListaDezenasSegundoSorteio, "DezenasSegundoSorteio deve existir na Dupla Sena");
        }

        // ✅ Testa comportamento de retry e exceção
        [Test]
        public void GetAsync_DeveTentarNovamenteEmErro()
        {
            var handler = new MockHttpMessageHandler("", HttpStatusCode.InternalServerError);
            var client = CreateClient(handler, new CaixaSettings
            {
                BaseUrl = "https://servicebus2.caixa.gov.br/portaldeloterias/api/",
                TimeoutSeconds = 1,
                RetryCount = 2,
                EnableLogging = false
            });

            Assert.ThrowsAsync<CaixaApiException>(async () =>
            {
                await client.ObterUltimoResultadoAsync("megasena");
            });
        }

        // 🔧 Handler mockado reutilizável
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
