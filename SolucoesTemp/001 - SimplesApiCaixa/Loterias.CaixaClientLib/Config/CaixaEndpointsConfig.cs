using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loterias.CaixaClientLib.Enums;

namespace Loterias.CaixaClientLib.Config
{
    public static class CaixaEndpointsConfig
    {
        public static readonly Dictionary<TipoLoteriaCaixa, string> Endpoints = new()
        {
            { TipoLoteriaCaixa.MegaSena, "megasena/" },
            { TipoLoteriaCaixa.Lotofacil, "lotofacil/" },
            { TipoLoteriaCaixa.Quina, "quina/" },
            { TipoLoteriaCaixa.Lotomania, "lotomania/" },
            { TipoLoteriaCaixa.Timemania, "timemania/" },
            { TipoLoteriaCaixa.DuplaSena, "duplasena/" },
            { TipoLoteriaCaixa.Federal, "federal/" },
            { TipoLoteriaCaixa.Loteca, "loteca/" },
            { TipoLoteriaCaixa.DiaDeSorte, "diadesorte/" },
            { TipoLoteriaCaixa.SuperSete, "supersete/" },
            { TipoLoteriaCaixa.MaisMilionaria, "maismilionaria/" }
        };

        public static string GetEndpoint(TipoLoteriaCaixa tipo)
        {
            if (Endpoints.TryGetValue(tipo, out var endpoint))
                return endpoint;
            throw new ArgumentException($"Tipo de loteria '{tipo}' não suportado pela API da Caixa.");
        }

        public static IEnumerable<TipoLoteriaCaixa> GetAll() => Endpoints.Keys;
    }
}

