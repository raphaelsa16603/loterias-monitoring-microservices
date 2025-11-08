using System.Text.Json.Serialization;

namespace Loterias.CaixaApiService.Models
{
    public sealed class MunicipioUFGanhadorDto
    {
        [JsonPropertyName("ganhadores")]
        public int? Ganhadores { get; set; }

        [JsonPropertyName("municipio")]
        public string? Municipio { get; set; }

        [JsonPropertyName("uf")]
        public string? UF { get; set; }

        [JsonPropertyName("posicao")]
        public int? Posicao { get; set; }

        [JsonPropertyName("serie")]
        public string? Serie { get; set; }

        [JsonPropertyName("nomeFatansiaUL")]
        public string? NomeFantasiaUL { get; set; }
    }
}
