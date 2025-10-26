using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Loterias.MonitoringWebApp.Services;

public class SeqLogService
{
    private readonly HttpClient _http;

    public SeqLogService(HttpClient http)
    {
        _http = http;
    }

    public async Task<JsonElement?> ObterLogsRecentesAsync()
    {
        var response = await _http.GetFromJsonAsync<JsonElement>("http://localhost:5341/api/events?count=10");
        return response;
    }
}
