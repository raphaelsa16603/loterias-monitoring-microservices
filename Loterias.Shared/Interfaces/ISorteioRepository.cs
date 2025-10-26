using Loterias.Shared.Models;

namespace Loterias.Shared.Interfaces;

public interface ISorteioRepository
{
    Task<Sorteio?> ObterPorNumeroAsync(string tipoJogo, int numeroJogo);
    Task<Sorteio?> ObterPorDataAsync(string tipoJogo, DateTime dataJogo);
    Task InserirAsync(Sorteio sorteio);
}
