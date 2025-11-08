using Loterias.CaixaApiService.Services;
using Loterias.CaixaApiService.Services.Interfaces;
using Loterias.CaixaApiService.Cache;
using Loterias.Shared;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Services;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Prometheus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;


public class Program
{
    public static void Main(string[] args)
    {



        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddSingleton<RedisCacheHandler>();

        // ---------- HTTP CLIENT (Caixa) ----------
        builder.Services.AddHttpClient<ICaixaApiClient, CaixaApiClient>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            var baseUrl = configuration["CaixaApi:BaseUrl"]
                ?? "https://servicebus2.caixa.gov.br/portaldeloterias/api/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(
                Convert.ToInt32(configuration["CaixaApi:TimeoutSeconds"] ?? "15"));
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        })
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1)));




        // ---------- SERVIÇOS ----------
        builder.Services.AddScoped<ICaixaApiService, CaixaApiService>();


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Loterias.CaixaApiService",
                Version = "v1",
                Description = "API intermediária entre o sistema e a Caixa Econômica Federal"
            });
        });

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        // ---------- MIDDLEWARE ----------
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // ⚠️ O routing precisa ser ativado ANTES dos endpoints
        app.UseRouting();

        // 🔧 Mapear métricas e health checks corretamente
        app.UseHttpMetrics(); // registra métricas automáticas de requisições HTTP

        // Mapeamentos de rotas de nível superior (recomendado pelo ASP0014)
        app.MapControllers();
        app.MapMetrics(); // expõe /metrics para Prometheus
        app.MapHealthChecks("/healthz"); // healthcheck para Docker

        // Health endpoint simples manual
        app.MapGet("/health", () => Results.Ok(new
        {
            status = "Healthy",
            redis = "Connected",
            version = "1.0.0"
        }));

        app.Run();
    }

}
