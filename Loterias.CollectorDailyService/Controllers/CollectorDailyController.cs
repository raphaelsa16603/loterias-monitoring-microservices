using Microsoft.AspNetCore.Mvc;

namespace Loterias.CollectorDailyService.Controllers;

[ApiController]
[Route("[controller]")]
public class CollectorDailyController : ControllerBase
{
    [HttpPost("execute")]
    public IActionResult Execute()
    {
        // Chamada ao serviço que executa a lógica real
        Console.WriteLine("Executando coleta diária...");
        return Ok("Coleta diária executada.");
    }
}
