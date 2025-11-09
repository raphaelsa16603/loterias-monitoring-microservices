using Loterias.CollectorDailyService.Services.Interfaces;

namespace Loterias.CollectorDailyService.Jobs
{
    public class CollectorDailyJob
    {
        private readonly ICollectorDailyService _collectorService;

        public CollectorDailyJob(ICollectorDailyService collectorService)
        {
            _collectorService = collectorService;
        }

        public async Task ExecutarAsync()
        {
            await _collectorService.ExecutarAsync();
        }
    }
}
