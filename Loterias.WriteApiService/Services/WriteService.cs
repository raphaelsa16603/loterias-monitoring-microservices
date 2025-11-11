using Loterias.Logging.Common.Interfaces;
using Loterias.Messaging.Interfaces;
using Loterias.Shared.DTOs;
using Loterias.Shared.Interfaces;
using Loterias.Shared.Models;
using Loterias.WriteApiService.Cache;
using Loterias.WriteApiService.Repositories;
using Loterias.WriteApiService.Services.Interfaces;

namespace Loterias.WriteApiService.Services
{
    public class WriteService : IWriteService
    {
        private readonly SorteioRepository _repo;
        private readonly RedisCacheHandler _cache;
        private readonly IStructuredLogger _logger;
        private readonly IMessageProducer _producer;

        public WriteService(SorteioRepository repo, RedisCacheHandler cache,
                            IStructuredLogger logger, IMessageProducer producer)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
            _producer = producer;
        }

        public async Task<ApiResponse<Sorteio>> UpsertSorteioAsync(Sorteio sorteio)
        {
            try
            {
                var updated = await _repo.UpsertAsync(sorteio);

                await _cache.SetAsync($"loterias:{sorteio.TipoLoteria}:ultimo", sorteio);

                await _producer.PublishAsync($"loterias.{sorteio.TipoLoteria.ToLower()}",
                    new { eventType = "SORTEIO_ATUALIZADO", payload = sorteio });

                _logger.Info($"WriteApiService - SorteioAtualizado - Sorteio {sorteio.TipoLoteria} {sorteio.Concurso} salvo com sucesso.");

                return ApiResponse<Sorteio>.Ok(updated, "Sorteio registrado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.Error($"WriteApiService - ErroGravacao {ex.Message}", ex);
                return ApiResponse<Sorteio>.Fail($"Erro ao salvar sorteio: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<Sorteio>>> UpsertBatchAsync(IEnumerable<Sorteio> sorteios)
        {
            foreach (var s in sorteios)
                await UpsertSorteioAsync(s);

            return ApiResponse<IEnumerable<Sorteio>>.Ok(sorteios, "Lote processado.");
        }

        public async Task RebuildCacheAsync()
        {
            var all = await _repo.ObterUltimosPorTipoAsync();
            foreach (var s in all)
                await _cache.SetAsync($"loterias:{s.TipoLoteria}:ultimo", s);
        }
    }
}
