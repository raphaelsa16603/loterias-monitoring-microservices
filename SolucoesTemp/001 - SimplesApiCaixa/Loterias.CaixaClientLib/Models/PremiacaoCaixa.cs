using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Loterias.CaixaClientLib.Models
{
    public class PremiacaoCaixa
    {
        [JsonPropertyName("descricaoFaixa")]
        public string DescricaoFaixa { get; set; } = string.Empty;

        [JsonPropertyName("faixa")]
        public int Faixa { get; set; }

        [JsonPropertyName("numeroDeGanhadores")]
        public int NumeroDeGanhadores { get; set; }

        [JsonPropertyName("valorPremio")]
        public decimal ValorPremio { get; set; }
    }
}

