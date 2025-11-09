using Loterias.CollectorDailyService.Services.Interfaces;
using Loterias.Logging.Common.Interfaces;

namespace Loterias.CollectorDailyService.Jobs
{
    public class CollectorDailyWorker : BackgroundService
    {
        private readonly ICollectorDailyService _collectorService;
        private readonly IStructuredLogger _logger;

        public CollectorDailyWorker(ICollectorDailyService collectorService, IStructuredLogger logger)
        {
            _collectorService = collectorService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("🚀 [CollectorDailyWorker] Serviço de coleta iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _collectorService.ExecutarAsync(stoppingToken);
                    _logger.Info("✅ [CollectorDailyWorker] Coleta concluída com sucesso.");
                }
                catch (Exception ex)
                {
                    _logger.Error($"❌ Erro na coleta: {ex.Message}", ex);
                }

                // aguarda 1 hora até a próxima execução (pode ajustar depois)
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.Warn("🛑 [CollectorDailyWorker] Serviço de coleta interrompido.");
        }
    }
}
