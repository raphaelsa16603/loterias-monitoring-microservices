using Microsoft.AspNetCore.Mvc;

namespace Loterias.QueryApiService.Controllers;

[ApiController]
[Route("api/sorteios")]
public class SorteiosController : ControllerBase
{
    [HttpGet("{tipoJogo}/{numeroJogo}")]
    public IActionResult GetByNumero(string tipoJogo, int numeroJogo)
    {
        return Ok(); // Implementar busca
    }

    [HttpGet("{tipoJogo}/data/{data}")]
    public IActionResult GetByData(string tipoJogo, DateTime data)
    {
        return Ok(); // Implementar busca
    }
}
