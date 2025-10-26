namespace Loterias.QueryApiService.Services;

using Loterias.Shared.Interfaces;
using Loterias.Shared.Models;

public class SorteioService : ISorteioService
{
    private readonly ISorteioRepository _repo;
    private readonly ICacheService _cache;

    public SorteioService(ISorteioRepository repo, ICacheService cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Sorteio?> ObterPorNumeroAsync(string tipoJogo, int numeroJogo)
    {
        var key = $"{tipoJogo}:{numeroJogo}";
        var cached = await _cache.GetAsync<Sorteio>(key);
        if (cached is not null) return cached;

        var result = await _repo.ObterPorNumeroAsync(tipoJogo, numeroJogo);
        if (result is not null) await _cache.SetAsync(key, result, TimeSpan.FromHours(1));
        return result;
    }

    public async Task<Sorteio?> ObterPorDataAsync(string tipoJogo, DateTime dataJogo)
    {
        var key = $"{tipoJogo}:{dataJogo:yyyy-MM-dd}";
        var cached = await _cache.GetAsync<Sorteio>(key);
        if (cached is not null) return cached;

        var result = await _repo.ObterPorDataAsync(tipoJogo, dataJogo);
        if (result is not null) await _cache.SetAsync(key, result, TimeSpan.FromHours(1));
        return result;
    }
}
