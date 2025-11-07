namespace Loterias.Shared.Interfaces;

using Loterias.Shared.Models;

public interface ISorteioService
{
    Task<Sorteio?> ObterPorNumeroAsync(string tipoJogo, int numeroJogo);
    Task<Sorteio?> ObterPorDataAsync(string tipoJogo, DateTime dataJogo);
}
