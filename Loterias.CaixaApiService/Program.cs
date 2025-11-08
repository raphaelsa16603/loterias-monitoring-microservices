using Loterias.CaixaApiService.Services;
using Loterias.CaixaApiService.Services.Interfaces;
using Loterias.CaixaApiService.Cache;
using Loterias.Shared;
using Loterias.CaixaClientLib.Interfaces;
using Loterias.CaixaClientLib.Services;
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Services;
using Loterias.Messaging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Polly;
using Polly.Extensions.Http;
using Prometheus;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Serilog;
using Serilog.Sinks.Graylog;

public class Program
{
    public static void Main(string[] args)
    {



        var builder = WebApplication.CreateBuilder(args);

        // ---------- CONFIGURAÇÕES BÁSICAS ----------
        builder.Host.UseSerilog((context, config) =>
        {
            config
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });

        // ---------- REDIS ----------
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConfig = builder.Configuration.GetSection("Redis");
            var host = redisConfig["Host"] ?? "loterias_redis";
            var port = redisConfig["Port"] ?? "6379";
            return ConnectionMultiplexer.Connect($"{host}:{port}");
        });

        builder.Services.AddSingleton<RedisCacheHandler>();

        // ---------- HTTP CLIENT (Caixa) ----------
        builder.Services.AddHttpClient<ICaixaApiClient, CaixaApiClient>(client =>
        {
            var baseUrl = builder.Configuration["CaixaApi:BaseUrl"]
                ?? "https://servicebus2.caixa.gov.br/portaldeloterias/api/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(
                Convert.ToInt32(builder.Configuration["CaixaApi:TimeoutSeconds"] ?? "15"));
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

        // 🔧 REGISTRO CORRETO DO LOGGER
        builder.Services.AddScoped<IStructuredLogger>(sp =>
        {
            // Nome do container Graylog e porta UDP configurada
            var graylogHost = "loterias-graylog";
            var graylogPort = 12201;
            var serviceName = "Loterias.CaixaApiService";

            return new StructuredLogger(graylogHost, graylogPort, serviceName);
        });


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
        app.MapControllers();
        app.MapMetrics(); // expõe /metrics para Prometheus
        app.MapHealthChecks("/healthz"); // healthcheck para Docker

        // Log estruturado de requisições HTTP
        app.UseSerilogRequestLogging();

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