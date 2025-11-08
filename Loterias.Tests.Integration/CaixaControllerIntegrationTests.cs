using System.Net;
using System.Net.Http.Json;
using Loterias.CaixaApiService;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Loterias.CaixaApiService.Tests.Integration
{
    public class CaixaControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CaixaControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Theory]
        [InlineData("megasena")]
        [InlineData("quina")]
        public async Task GetUltimoAsync_DeveRetornarOkOuNotFound(string tipo)
        {
            // Act
            var response = await _client.GetAsync($"/api/v1/caixa/{tipo}/ultimo");

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound,
                $"StatusCode retornado ({response.StatusCode}) não é OK nem NotFound para o tipo '{tipo}'"
            );
        }

        [Theory]
        [InlineData("megasena", 2663)]
        [InlineData("quina", 1234)]
        public async Task GetConcursoAsync_DeveRetornarOkOuNotFound(string tipo, int concurso)
        {
            // Act
            var response = await _client.GetAsync($"/api/v1/caixa/{tipo}/{concurso}");

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound,
                $"StatusCode retornado ({response.StatusCode}) não é OK nem NotFound para o tipo '{tipo}', concurso {concurso}"
            );
        }

        [Theory]
        [InlineData("megasena")]
        [InlineData("quina")]
        public async Task RefreshAsync_DeveRetornarOkOuNotFound(string tipo)
        {
            // Act
            var response = await _client.GetAsync($"/api/v1/caixa/refresh/{tipo}");

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound,
                $"StatusCode retornado ({response.StatusCode}) não é OK nem NotFound para o tipo '{tipo}'"
            );
        }
    }
}
