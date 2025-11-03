using Loterias.CaixaApiService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Loterias.CaixaApiService.Controllers
{
    [ApiController]
    [Route("api/v1/caixa")]
    public class CaixaController : ControllerBase
    {
        private readonly ICaixaApiService _service;

        public CaixaController(ICaixaApiService service)
        {
            _service = service;
        }

        [HttpGet("{tipo}/ultimo")]
        public async Task<IActionResult> GetUltimoAsync(string tipo)
        {
            var resultado = await _service.ObterUltimoAsync(tipo);
            return resultado is null ? NotFound() : Ok(resultado);
        }

        [HttpGet("{tipo}/{concurso:int}")]
        public async Task<IActionResult> GetConcursoAsync(string tipo, int concurso)
        {
            var resultado = await _service.ObterPorConcursoAsync(tipo, concurso);
            return resultado is null ? NotFound() : Ok(resultado);
        }

        [HttpGet("refresh/{tipo}")]
        public async Task<IActionResult> RefreshAsync(string tipo)
        {
            var atualizado = await _service.AtualizarCacheAsync(tipo);
            return atualizado is null ? NotFound() : Ok(atualizado);
        }
    }
}
