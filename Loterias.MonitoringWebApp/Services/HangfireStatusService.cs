using System.Net.Http;
using System.Threading.Tasks;

namespace Loterias.MonitoringWebApp.Services;

public class HangfireStatusService
{
    private readonly HttpClient _http;

    public HangfireStatusService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> ObterHtmlDashboardAsync()
    {
        var html = await _http.GetStringAsync("http://localhost:5005/jobs");
        return html;
    }
}
