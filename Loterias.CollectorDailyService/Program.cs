using Loterias.CollectorDailyService.Jobs;
using Loterias.CollectorDailyService.Services;
using Loterias.CollectorDailyService.Services.Interfaces;
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Services;
using Loterias.Messaging;
using Loterias.Messaging.Interfaces;
using Loterias.Messaging.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Prometheus;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // 🔹 HttpClient Factory (necessário p/ CaixaClientLib)
        services.AddHttpClient();

        // 🔹 Configurações do Kafka (com suporte a IOptions<KafkaSettings>)
        services.Configure<KafkaSettings>(opts =>
        {
            opts.BootstrapServers = "kafka:9092";
            opts.BaseTopicName = "loterias";
            opts.RetryCount = 3;
            opts.PublishTimeoutMs = 3000;
        });

        // 🔹 Garante que serviços que usam KafkaSettings direto também funcionem
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

        // 🔹 Logging estruturado com Graylog
        services.AddSingleton<IStructuredLogger>(sp =>
        {
            var graylogHost = "loterias-graylog";
            var graylogPort = 12201;
            var serviceName = "Loterias.CollectorDailyService";

            // 🚀 A classe StructuredLogger deve ter um construtor compatível
            return new StructuredLogger(graylogHost, graylogPort, serviceName);
        });

        // 🔹 Mensageria Kafka
        services.AddSingleton<IMessageProducer, KafkaProducer>();

        // 🔹 Serviço principal do Collector
        services.AddSingleton<ICollectorDailyService, CollectorDailyService>();

        // 🔹 Worker em background
        services.AddHostedService<CollectorDailyWorker>();

        // 🔹 Métricas Prometheus (opcional, caso queira expor /metrics)
        services.AddSingleton<ICollectorRegistry>(Metrics.DefaultRegistry);
    })
    .UseSerilog((context, config) =>
    {
        config
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    });

// 🔹 Inicia o worker como aplicação de console
await builder.RunConsoleAsync();
