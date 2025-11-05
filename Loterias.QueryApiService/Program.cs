// Program.cs
using Loterias.QueryApiService.Repositories;
using Loterias.QueryApiService.Services;
using Loterias.RedisCacheService.Cache;
using Loterias.Shared.Interfaces;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using Prometheus;

namespace Loterias.QueryApiService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers();

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Loterias Query API",
                Version = "v1",
                Description = "API para consulta de resultados das Loterias da Caixa"
            });
        });

        // MongoDB (usa seu appsettings.MongoDB)
        var mongoConn = builder.Configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
        var mongoDbName = builder.Configuration["MongoDB:Database"] ?? "Loterias";
        builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
        builder.Services.AddScoped<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDbName));

        // Redis (alinha versão do pacote!)
        var redisConn = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
        builder.Services.AddSingleton<ICacheService, Loterias.RedisCacheService.Cache.RedisCacheService>();

        // Repositório + Serviço de domínio
        builder.Services.AddScoped<ISorteioRepository, SorteioMongoRepository>();
        builder.Services.AddScoped<ISorteioService, SorteioService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        app.MapGet("/health", () => Results.Ok("OK"));

        app.MapMetrics(); // Prometheus metrics endpoint // expõe /metrics

        app.Run();
    }
}
