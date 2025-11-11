using Loterias.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/loterias")]
public class LoteriasController : ControllerBase
{
    private readonly ISorteioService _service;

    public LoteriasController(ISorteioService service) => _service = service;

    [HttpGet("{tipo}/{numero:int}")]
    public async Task<IActionResult> GetByNumero(string tipo, int numero)
    {
        try
        {
            var sorteio = await _service.ObterPorNumeroAsync(tipo, numero);
            return sorteio is null ? NotFound() : Ok(sorteio);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno", detail = ex.Message });
        }
    }

    [HttpGet("{tipo}/data/{data:datetime}")]
    public async Task<IActionResult> GetByData(string tipo, DateTime data)
    {
        try
        {
            var sorteio = await _service.ObterPorDataAsync(tipo, data);
            return sorteio is null ? NotFound() : Ok(sorteio);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno", detail = ex.Message });
        }
    }
}
