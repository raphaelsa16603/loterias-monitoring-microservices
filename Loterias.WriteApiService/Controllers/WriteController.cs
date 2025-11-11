using Loterias.Shared.Models;
using Loterias.WriteApiService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Loterias.WriteApiService.Controllers
{
    [ApiController]
    [Route("api/v1/write")]
    public class WriteController : ControllerBase
    {
        private readonly IWriteService _service;

        public WriteController(IWriteService service) => _service = service;

        [HttpPost("sorteios")]
        public async Task<IActionResult> InsertAsync([FromBody] Sorteio sorteio)
        {
            var result = await _service.UpsertSorteioAsync(sorteio);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("lotes")]
        public async Task<IActionResult> InsertBatchAsync([FromBody] IEnumerable<Sorteio> sorteios)
        {
            var result = await _service.UpsertBatchAsync(sorteios);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("cache/rebuild")]
        public async Task<IActionResult> RebuildCacheAsync()
        {
            await _service.RebuildCacheAsync();
            return Ok(new { success = true, message = "Cache Redis atualizado." });
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "WriteApiService OK" });
    }
}
