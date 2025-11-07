using System.Net;
using System.Net.Http.Json;
using Loterias.CaixaApiService;
using Loterias.CaixaApiService.Controllers;
using Loterias.CaixaApiService.Services.Interfaces;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Loterias.CaixaApiService.Tests.Integration
{
    [TestFixture]
    public class CaixaControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        [TestCase("megasena")]
        [TestCase("quina")]
        public async Task GetUltimoAsync_DeveRetornarOkOuNotFound(string tipo)
        {
            var response = await _client.GetAsync($"/api/v1/caixa/{tipo}/ultimo");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [TestCase("megasena", 2663)]
        [TestCase("quina", 1234)]
        public async Task GetConcursoAsync_DeveRetornarOkOuNotFound(string tipo, int concurso)
        {
            var response = await _client.GetAsync($"/api/v1/caixa/{tipo}/{concurso}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [TestCase("megasena")]
        [TestCase("quina")]
        public async Task RefreshAsync_DeveRetornarOkOuNotFound(string tipo)
        {
            var response = await _client.GetAsync($"/api/v1/caixa/refresh/{tipo}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound));
        }
    }
}