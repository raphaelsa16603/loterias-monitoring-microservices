using Loterias.CollectorDailyService.Services.Interfaces;
using Loterias.Logging.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Loterias.CollectorDailyService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CollectorDailyController : ControllerBase
    {
        private readonly ICollectorDailyService _collectorService;
        private readonly IStructuredLogger _logger;

        public CollectorDailyController(
            ICollectorDailyService collectorService,
            IStructuredLogger logger)
        {
            _collectorService = collectorService;
            _logger = logger;
        }

        /// <summary>
        /// Executa manualmente a coleta diária (útil para testes ou debug)
        /// </summary>
        [HttpPost("execute")]
        public async Task<IActionResult> Execute()
        {
            _logger.Info("Recebida requisição manual para execução da coleta diária.");
            await _collectorService.ExecutarAsync();
            return Ok(new { message = "Coleta diária executada com sucesso." });
        }

        /// <summary>
        /// Endpoint básico de healthcheck
        /// </summary>
        [HttpGet("healthz")]
        public IActionResult HealthCheck() => Ok("CollectorDailyService ativo e operacional");
    }
}
