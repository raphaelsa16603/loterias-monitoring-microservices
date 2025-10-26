using Microsoft.AspNetCore.Mvc;

namespace Loterias.CollectorHistoricalService.Controllers;

[ApiController]
[Route("[controller]")]
public class CollectorHistoricalController : ControllerBase
{
    [HttpPost("execute")]
    public IActionResult Execute()
    {
        // Chamada ao serviço que executa a lógica real
        Console.WriteLine("Executando coleta histórica...");
        return Ok("Coleta histórica executada.");
    }
}
