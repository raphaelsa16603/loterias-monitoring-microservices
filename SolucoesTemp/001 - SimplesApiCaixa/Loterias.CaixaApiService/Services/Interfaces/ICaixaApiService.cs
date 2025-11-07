using Loterias.CaixaApiService.Models;

namespace Loterias.CaixaApiService.Services.Interfaces
{
    public interface ICaixaApiService
    {
        Task<CaixaResponseDto?> ObterUltimoAsync(string tipo);
        Task<CaixaResponseDto?> ObterPorConcursoAsync(string tipo, int concurso);
        Task<CaixaResponseDto?> AtualizarCacheAsync(string tipo);
    }
}
