using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loterias.CaixaClientLib.Config;
using Loterias.CaixaClientLib.Enums;

namespace Loterias.CaixaClientLib.Services
{
    public class CaixaEndpointsProvider
    {
        private readonly string _baseUrl;

        public CaixaEndpointsProvider(string baseUrl)
        {
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
        }

        public string GetUrlUltimoResultado(TipoLoteriaCaixa tipo)
        {
            var endpoint = CaixaEndpointsConfig.GetEndpoint(tipo);
            return $"{_baseUrl}{endpoint}ultimo";
        }

        public string GetUrlPorConcurso(TipoLoteriaCaixa tipo, int concurso)
        {
            var endpoint = CaixaEndpointsConfig.GetEndpoint(tipo);
            return $"{_baseUrl}{endpoint}{concurso}";
        }

        public IEnumerable<string> GetAllUltimosResultadosUrls()
        {
            foreach (var tipo in CaixaEndpointsConfig.GetAll())
                yield return GetUrlUltimoResultado(tipo);
        }
    }
}

