using Xunit;
using System.Net.Http;
using System.Threading.Tasks;

namespace Loterias.Tests.Integration;

public class QueryApiIntegrationTests
{
    [Fact]
    public async Task GetByNumero_DeveRetornarStatusCode200()
    {
        using var client = new HttpClient();
        var baseUrl = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5341";
        var response = await client.GetAsync($"{baseUrl}/api/sorteios/MEGA_SENA/1234");

        Assert.True(response.IsSuccessStatusCode);
    }
}
