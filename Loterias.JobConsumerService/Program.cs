using Loterias.JobConsumerService.Config;
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Services;
using Loterias.Messaging.Interfaces;
using Loterias.Messaging.Kafka;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Loterias.JobConsumerService.Services;
using Loterias.JobConsumerService;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Extensions.Hosting; // Adicione esta linha


var builder = Host.CreateDefaultBuilder(args);



builder.ConfigureServices((context, services) =>
{
    var config = context.Configuration;

    services.Configure<JobConsumerSettings>(config.GetSection("Kafka"));
    services.Configure<HttpSettings>(config.GetSection("WriteApi"));

    services.AddHttpClient(); // Para chamadas HTTP ao WriteApiService

    services.AddSingleton<IStructuredLogger, StructuredLogger>();
    services.AddSingleton<IMessageConsumer, KafkaConsumer>();
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
