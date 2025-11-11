using Loterias.Logging.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Loterias.JobConsumerService
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IStructuredLogger _logger;

        public Worker(IServiceProvider serviceProvider, IStructuredLogger logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("JobConsumerService - WorkerStart - Iniciando consumo de tópicos Kafka...");

            using var scope = _serviceProvider.CreateScope();
            var executor = scope.ServiceProvider.GetRequiredService<Services.JobExecutorService>();
            await executor.StartAsync(stoppingToken);
        }
    }
}
