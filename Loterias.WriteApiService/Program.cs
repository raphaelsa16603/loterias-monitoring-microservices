using Loterias.WriteApiService.Cache;
using Loterias.WriteApiService.Config;
using Loterias.WriteApiService.Repositories;
using Loterias.WriteApiService.Services;
using Loterias.WriteApiService.Services.Interfaces;
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Services;
using Loterias.Messaging.Interfaces;
using Loterias.Messaging.Kafka;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;
using Serilog;
using Serilog.Sinks.Graylog;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configuração de Serilog + Graylog
// ---------- CONFIGURAÇÕES BÁSICAS ----------
builder.Host.UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

// 🔹 Configuração
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
builder.Services.Configure<Loterias.Messaging.Kafka.KafkaSettings>(builder.Configuration.GetSection("Kafka"));

// 🔹 MongoDB
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});

// 🔹 Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redis = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    return ConnectionMultiplexer.Connect($"{redis.Host}:{redis.Port}");
});

// 🔹 Serviços internos
builder.Services.AddScoped<SorteioRepository>();
builder.Services.AddScoped<RedisCacheHandler>();
builder.Services.AddScoped<IWriteService, WriteService>();

// 🔹 Logging e Kafka
// 🔹 Logging e Kafka
builder.Services.AddSingleton<IStructuredLogger>(sp =>
{
    var graylogHost = "loterias-graylog";
    var graylogPort = 12201;
    var serviceName = "Loterias.WriteApiService";
    return new StructuredLogger(graylogHost, graylogPort, serviceName);
});

// 🔹 Configuração Kafka
builder.Services.Configure<KafkaSettings>(opts =>
{
    // usa o nome do container do Kafka no Docker, e não localhost
    opts.BootstrapServers = "kafka:9092";
    opts.BaseTopicName = "loterias";
    opts.RetryCount = 3;
    opts.PublishTimeoutMs = 3000;
});

// 🔹 Injeta KafkaSettings via Options
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

// ✅ REGISTRA O PRODUTOR KAFKA
builder.Services.AddSingleton<IMessageProducer, KafkaProducer>();

// 🔹 Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// 🔹 Middleware Prometheus
app.UseHttpMetrics();

// 🔹 Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
