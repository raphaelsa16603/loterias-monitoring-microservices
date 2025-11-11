using Loterias.Shared.DTOs;
using Loterias.Shared.Models;

namespace Loterias.WriteApiService.Services.Interfaces
{
    public interface IWriteService
    {
        Task<ApiResponse<Sorteio>> UpsertSorteioAsync(Sorteio sorteio);
        Task<ApiResponse<IEnumerable<Sorteio>>> UpsertBatchAsync(IEnumerable<Sorteio> sorteios);
        Task RebuildCacheAsync();
    }
}
