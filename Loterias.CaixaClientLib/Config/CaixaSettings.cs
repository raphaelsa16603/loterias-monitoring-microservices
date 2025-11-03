using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loterias.CaixaClientLib.Config
{
    public class CaixaSettings
    {
        public string BaseUrl { get; set; } = "https://servicebus2.caixa.gov.br/portaldeloterias/api/";
        public int TimeoutSeconds { get; set; } = 10;
        public int RetryCount { get; set; } = 3;
        public bool EnableLogging { get; set; } = true;
        public string? AlternateUrl { get; set; } // URL secundária caso o endpoint principal caia
    }
}

