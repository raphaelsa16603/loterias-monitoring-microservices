using Hangfire;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Loterias.JobControlService.Configurations;

public static class HangfireJobs
{
    private static readonly HttpClient httpClient = new();

    public static void Register()
    {
        RecurringJob.AddOrUpdate("collector-daily", () => ExecutarCollector("collector-daily"), Cron.Daily(0, 1));
        RecurringJob.AddOrUpdate("collector-historical", () => ExecutarCollector("collector-historical"), "*/15 4-9 * * *");
    }

    public static async Task ExecutarCollector(string tipo)
    {
        var url = tipo switch
        {
            "collector-daily" => "http://localhost:5011/execute",
            "collector-historical" => "http://localhost:5012/execute",
            _ => throw new ArgumentException("Tipo inválido")
        };

        var response = await httpClient.PostAsync(url, new StringContent("", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }
}
