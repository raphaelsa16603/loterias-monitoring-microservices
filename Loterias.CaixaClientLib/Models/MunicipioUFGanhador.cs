using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Loterias.CaixaClientLib.Models
{
    public class MunicipioUFGanhador
    {
        [JsonPropertyName("ganhadores")]
        public int? Ganhadores { get; set; }

        [JsonPropertyName("municipio")]
        public string Municipio { get; set; } = string.Empty;

        [JsonPropertyName("uf")]
        public string UF { get; set; } = string.Empty;

        [JsonPropertyName("posicao")]
        public int? Posicao { get; set; }

        [JsonPropertyName("serie")]
        public string? Serie { get; set; }

        [JsonPropertyName("nomeFatansiaUL")]
        public string? NomeFantasiaUL { get; set; }
    }
}
