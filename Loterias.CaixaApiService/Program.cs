using Loterias.CaixaApiService.Services;
using Loterias.CaixaApiService.Services.Interfaces;
using Loterias.CaixaApiService.Cache;
using Loterias.Shared;
using Loterias.CaixaClientLib.Interfaces;  // ✅ certo agora
using Loterias.CaixaClientLib.Services;    // ✅ implementação
using Loterias.Logging.Common;
using Loterias.Messaging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Polly;
using Polly.Extensions.Http;               // ✅ necessário para AddTransientHttpErrorPolicy
using Serilog;
using Prometheus;                           // Para métricas Prometheus
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.AspNetCore; // Adicione este using para acessar o método de extensão UseSerilogRequestLogging
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Services;
using Microsoft.Extensions.Http; // Necessário para AddPolicyHandler
using Microsoft.Extensions.DependencyInjection; // NECESSÁRIO para AddPolicyHandler



var builder = WebApplication.CreateBuilder(args);

// ---------- CONFIGURAÇÕES BÁSICAS ----------
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
    config.WriteTo.Console();
});

// ---------- REDIS ----------
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConfig = builder.Configuration.GetSection("Redis");
    var host = redisConfig["Host"];
    var port = redisConfig["Port"];
    return ConnectionMultiplexer.Connect($"{host}:{port}");
});

builder.Services.AddSingleton<RedisCacheHandler>();

// ---------- HTTP CLIENT (Caixa) ----------
builder.Services.AddHttpClient<ICaixaApiClient, CaixaApiClient>(client =>
{
    var baseUrl = builder.Configuration["CaixaApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
    client.Timeout = TimeSpan.FromSeconds(
        Convert.ToInt32(builder.Configuration["CaixaApi:TimeoutSeconds"] ?? "15"));
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1)));


// ---------- SERVIÇOS ----------
builder.Services.AddScoped<ICaixaApiService, CaixaApiService>();
builder.Services.AddScoped<IStructuredLogger, StructuredLogger>();

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

var app = builder.Build();

// ---------- MIDDLEWARE ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapMetrics(); // expõe /metrics
});

app.MapHealthChecks("/healthz");


app.UseSerilogRequestLogging();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    redis = "Connected",
    version = "1.0.0"
}));

app.Run();
