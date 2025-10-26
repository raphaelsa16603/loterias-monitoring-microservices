using Loterias.Shared.Models;

namespace Loterias.Shared.Interfaces;

public interface ICaixaApiClient
{
    Task<string?> BuscarResultadoPorNumeroAsync(string tipoJogo, int numeroJogo);
    Task<string?> BuscarResultadoPorDataAsync(string tipoJogo, DateTime data);
}
