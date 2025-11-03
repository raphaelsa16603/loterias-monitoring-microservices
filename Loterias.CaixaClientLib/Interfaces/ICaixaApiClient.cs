using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Loterias.CaixaClientLib.Enums;
using Loterias.CaixaClientLib.Models;

namespace Loterias.CaixaClientLib.Interfaces
{
    public interface ICaixaApiClient
    {
        // Consulta pelo nome textual (ex: "megasena")
        Task<CaixaResponse?> ObterUltimoResultadoAsync(string tipoLoteria);
        Task<CaixaResponse?> ObterResultadoPorConcursoAsync(string tipoLoteria, int concurso);

        // Consulta fortemente tipada (enum)
        Task<CaixaResponse?> ObterUltimoResultadoAsync(TipoLoteriaCaixa tipo);
        Task<CaixaResponse?> ObterResultadoPorConcursoAsync(TipoLoteriaCaixa tipo, int concurso);
    }
}

