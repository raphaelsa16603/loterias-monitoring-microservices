namespace Loterias.Shared.Models;

public class Sorteio
{
    public int NumeroJogo { get; set; }
    public DateTime DataJogo { get; set; }
    public string TipoJogo { get; set; } = string.Empty;
    public string JsonCompleto { get; set; } = string.Empty;
}
