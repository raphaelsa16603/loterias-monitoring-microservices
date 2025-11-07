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
using System.Net.Http.Json;
using System.Text.Json;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace TestDaApiCaixa
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

        [Test]
        public async Task ObterUltimoResultadoAsync_String_DeveRetornarCaixaResponse()
        {
            var expected = new CaixaResponse { TipoJogo = "megasena", NumeroConcurso = 1234 };
            var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(expected), HttpStatusCode.OK);
            var client = CreateClient(handler);

            var result = await client.ObterUltimoResultadoAsync("megasena");

            Assert.NotNull(result);
            Assert.AreEqual("megasena", result!.TipoJogo);
            Assert.AreEqual(1234, result.NumeroConcurso);
        }

        [Test]
        public async Task ObterResultadoPorConcursoAsync_String_DeveRetornarCaixaResponse()
        {
            var expected = new CaixaResponse { TipoJogo = "quina", NumeroConcurso = 5678 };
            var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(expected), HttpStatusCode.OK);
            var client = CreateClient(handler);

            var result = await client.ObterResultadoPorConcursoAsync("quina", 5678);

            Assert.NotNull(result);
            Assert.AreEqual("quina", result!.TipoJogo);
            Assert.AreEqual(5678, result.NumeroConcurso);
        }

        [Test]
        public async Task ObterUltimoResultadoAsync_Enum_DeveRetornarCaixaResponse()
        {
            var expected = new CaixaResponse { TipoJogo = "lotofacil", NumeroConcurso = 9999 };
            var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(expected), HttpStatusCode.OK);
            var client = CreateClient(handler);

            var result = await client.ObterUltimoResultadoAsync(TipoLoteriaCaixa.Lotofacil);

            Assert.NotNull(result);
            Assert.AreEqual("lotofacil", result!.TipoJogo);
            Assert.AreEqual(9999, result.NumeroConcurso);
        }

        [Test]
        public async Task ObterResultadoPorConcursoAsync_Enum_DeveRetornarCaixaResponse()
        {
            var expected = new CaixaResponse { TipoJogo = "timemania", NumeroConcurso = 4321 };
            var handler = new MockHttpMessageHandler(JsonSerializer.Serialize(expected), HttpStatusCode.OK);
            var client = CreateClient(handler);

            var result = await client.ObterResultadoPorConcursoAsync(TipoLoteriaCaixa.Timemania, 4321);

            Assert.NotNull(result);
            Assert.AreEqual("timemania", result!.TipoJogo);
            Assert.AreEqual(4321, result.NumeroConcurso);
        }

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