using Loterias.Shared.Interfaces;
using Loterias.Shared.Models;
using MongoDB.Driver;

namespace Loterias.QueryApiService.Repositories;

public class SorteioMongoRepository : ISorteioRepository
{
    private readonly IMongoCollection<Sorteio> _collection;

    public SorteioMongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Sorteio>("Sorteios");
    }

    public async Task<Sorteio?> ObterPorNumeroAsync(string tipoJogo, int numeroJogo)
    {
        return await _collection.Find(s => s.TipoLoteria == tipoJogo && s.Concurso == numeroJogo).FirstOrDefaultAsync();
    }

    public async Task<Sorteio?> ObterPorDataAsync(string tipoJogo, DateTime dataJogo)
    {
        return await _collection.Find(s => s.TipoLoteria == tipoJogo && s.DataSorteio == dataJogo).FirstOrDefaultAsync();
    }

    public async Task InserirAsync(Sorteio sorteio)
    {
        await _collection.InsertOneAsync(sorteio);
    }
}
