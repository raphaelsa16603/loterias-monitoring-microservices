// Loterias.QueryApiService/Controllers/SorteiosController.cs
using Loterias.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Loterias.QueryApiService.Controllers;

[ApiController]
[Route("api/sorteios")]
public class SorteiosController : ControllerBase
{
    private readonly ISorteioService _service;

    public SorteiosController(ISorteioService service) => _service = service;

    [HttpGet("{tipoJogo}/{numeroJogo:int}")]
    public async Task<IActionResult> GetByNumero(string tipoJogo, int numeroJogo)
    {
        try
        {
            var s = await _service.ObterPorNumeroAsync(tipoJogo, numeroJogo);
            return s is null ? NotFound() : Ok(s);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno", detail = ex.Message });
        }
    }

    [HttpGet("{tipoJogo}/data/{data:datetime}")]
    public async Task<IActionResult> GetByData(string tipoJogo, DateTime data)
    {
        try
        {
            var s = await _service.ObterPorDataAsync(tipoJogo, data);
            return s is null ? NotFound() : Ok(s);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno", detail = ex.Message });
        }
    }
}
