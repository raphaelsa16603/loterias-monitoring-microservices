using Loterias.Shared.Models; // já existe no seu Core/Shared
using MongoDB.Bson;
using MongoDB.Driver;

namespace Loterias.WriteApiService.Repositories
{
    public class SorteioRepository
    {
        private readonly IMongoCollection<Sorteio> _collection;

        public SorteioRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Sorteio>("Sorteios");

            // Índices importantes para upsert/consulta
            var idxKeys = Builders<Sorteio>.IndexKeys
                .Ascending(s => s.TipoLoteria)
                .Ascending(s => s.Concurso);

            var uniqueIndex = new CreateIndexModel<Sorteio>(
                idxKeys,
                new CreateIndexOptions { Unique = true, Name = "uk_tipo_concurso" });

            _collection.Indexes.CreateOne(uniqueIndex);

            var dataIdx = new CreateIndexModel<Sorteio>(
                Builders<Sorteio>.IndexKeys.Descending(s => s.DataSorteio),
                new CreateIndexOptions { Name = "ix_data" });

            _collection.Indexes.CreateOne(dataIdx);
        }

        public async Task<Sorteio> UpsertAsync(Sorteio sorteio, CancellationToken ct = default)
        {
            // filtro por chave natural (tipo + concurso)
            var filter = Builders<Sorteio>.Filter.And(
                Builders<Sorteio>.Filter.Eq(s => s.TipoLoteria, sorteio.TipoLoteria),
                Builders<Sorteio>.Filter.Eq(s => s.Concurso, sorteio.Concurso)
            );

            var options = new ReplaceOptions { IsUpsert = true };
            await _collection.ReplaceOneAsync(filter, sorteio, options, ct);
            return sorteio;
        }

        public async Task<IReadOnlyList<Sorteio>> ObterUltimosPorTipoAsync(string? tipo = null, int take = 10, CancellationToken ct = default)
        {
            var filter = tipo is null
                ? FilterDefinition<Sorteio>.Empty
                : Builders<Sorteio>.Filter.Eq(s => s.TipoLoteria, tipo);

            var cursor = await _collection.Find(filter)
                                          .SortByDescending(s => s.DataSorteio)
                                          .Limit(take)
                                          .ToListAsync(ct);

            return cursor;
        }

        public async Task<bool> DeleteAsync(string tipo, int concurso, CancellationToken ct = default)
        {
            var filter = Builders<Sorteio>.Filter.And(
                Builders<Sorteio>.Filter.Eq(s => s.TipoLoteria, tipo),
                Builders<Sorteio>.Filter.Eq(s => s.Concurso, concurso)
            );

            var res = await _collection.DeleteOneAsync(filter, ct);
            return res.DeletedCount > 0;
        }
    }
}
