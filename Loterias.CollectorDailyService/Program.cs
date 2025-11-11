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
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;


        // 🔹 HttpClients nomeados com BaseAddress configurada
        services.AddHttpClient("CaixaApi", c =>
        {
            c.BaseAddress = new Uri("http://localhost:5002/");
            c.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddHttpClient("QueryApi", c =>
        {
            c.BaseAddress = new Uri("http://localhost:5003/");
            c.Timeout = TimeSpan.FromSeconds(15);
        });


        // 🔹 Configurações do Kafka (localhost em vez de container)
        services.Configure<KafkaSettings>(opts =>
        {
            opts.BootstrapServers = "localhost:9092";
            opts.BaseTopicName = "loterias";
            opts.RetryCount = 3;
            opts.PublishTimeoutMs = 3000;
        });

        // 🔹 Garante compatibilidade para injeção direta
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

        // 🔹 Logging estruturado (Graylog local)
        services.AddSingleton<IStructuredLogger>(sp =>
        {
            var graylogHost = "localhost"; // antes: "loterias-graylog"
            var graylogPort = 12201;
            var serviceName = "Loterias.CollectorDailyService";

            return new StructuredLogger(graylogHost, graylogPort, serviceName);
        });

        // 🔹 Mensageria Kafka
        services.AddSingleton<IMessageProducer, KafkaProducer>();

        // 🔹 Serviço principal do Collector
        services.AddSingleton<ICollectorDailyService, CollectorDailyService>();

        // 🔹 Worker em background
        services.AddHostedService<CollectorDailyWorker>();

        // 🔹 Métricas Prometheus (opcional)
        services.AddSingleton<ICollectorRegistry>(Metrics.DefaultRegistry);
    })
    .UseSerilog((context, config) =>
    {
        config
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    });


// ...toda a sua configuração acima
var host = builder.Build();
await host.RunAsync();

