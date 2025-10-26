using MongoDB.Driver;
using Loterias.Shared.Models;

namespace Loterias.MonitoringWebApp.Services;

public class SorteioService
{
    private readonly IMongoCollection<Sorteio> _collection;

    public SorteioService(IMongoClient client)
    {
        var db = client.GetDatabase("Loterias");
        _collection = db.GetCollection<Sorteio>("Sorteios");
    }

    public async Task<List<Sorteio>> ObterUltimosSorteiosAsync(int quantidade = 10)
    {
        return await _collection.Find(_ => true)
            .SortByDescending(s => s.DataJogo)
            .Limit(quantidade)
            .ToListAsync();
    }
}
