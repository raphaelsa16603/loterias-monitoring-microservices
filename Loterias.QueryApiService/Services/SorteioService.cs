using Loterias.Shared.Models;

namespace Loterias.QueryApiService.Services;

public class SorteioService
{
    public Task<Sorteio?> ObterPorNumeroAsync(string tipoJogo, int numeroJogo)
    {
        return Task.FromResult<Sorteio?>(null);
    }

    public Task<Sorteio?> ObterPorDataAsync(string tipoJogo, DateTime data)
    {
        return Task.FromResult<Sorteio?>(null);
    }
}
