namespace Loterias.CollectorDailyService.Services.Interfaces
{
    public interface ICollectorDailyService
    {
        Task ExecutarAsync(CancellationToken cancellationToken = default);
    }
}
