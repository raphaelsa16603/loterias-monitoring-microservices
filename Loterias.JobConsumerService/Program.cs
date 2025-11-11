using Loterias.JobConsumerService;
using Loterias.JobConsumerService.Config;
using Loterias.JobConsumerService.Services;
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Services;
using Loterias.Messaging.Interfaces;
using Loterias.Messaging.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting; // Adicione esta linha
using Serilog.Formatting.Compact;


var builder = Host.CreateDefaultBuilder(args);



builder.ConfigureServices((context, services) =>
{
    var configuration = context.Configuration;

    // 🔹 Bind correto da seção Kafka
    services.Configure<JobConsumerSettings>(configuration.GetSection("Kafka"));
    services.Configure<HttpSettings>(configuration.GetSection("WriteApi"));

    services.AddHttpClient(); // Para chamadas HTTP ao WriteApiService

    services.AddSingleton<IStructuredLogger>(sp =>
    {
        var graylogHost = "localhost";
        var graylogPort = 12201;
        var serviceName = "Loterias.WriteApiService";
        return new StructuredLogger(graylogHost, graylogPort, serviceName);
    });

    // 🔹 Configuração Kafka
    services.Configure<KafkaSettings>(opts =>
    {
        // usa o nome do container do Kafka no Docker, e não localhost
        opts.BootstrapServers = "localhost:9092";
        opts.BaseTopicName = "loterias";
        opts.RetryCount = 3;
        opts.PublishTimeoutMs = 3000;
    });

    // 🔹 Injeta KafkaSettings via Options
    services.AddSingleton(sp =>
        sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

    // ✅ REGISTRA O PRODUTOR KAFKA
    services.AddSingleton<IMessageProducer, KafkaProducer>();

    services.AddScoped<Loterias.JobConsumerService.Consumers.SorteiosConsumer>(); // << -- Adicionado

    services.AddHostedService<Worker>();
    
    services.AddScoped<JobExecutorService>();

    

}).UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

await builder.Build().RunAsync();
