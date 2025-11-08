using Loterias.CaixaClientLib.Util;
using System;
using System.Text.Json.Serialization;

namespace Loterias.CaixaClientLib.Models
{
    public class ResultadoEquipeEsportiva
    {
        [JsonPropertyName("diaSemana")]
        public string? DiaSemana { get; set; }

        [JsonPropertyName("dtJogo")]
        [JsonConverter(typeof(DateTimeConverterCaixa))]
        public DateTime? DataJogo { get; set; }

        [JsonPropertyName("nomeCampeonato")]
        public string? Campeonato { get; set; }

        [JsonPropertyName("nomeEquipeUm")]
        public string? EquipeUm { get; set; }

        [JsonPropertyName("nomeEquipeDois")]
        public string? EquipeDois { get; set; }

        [JsonPropertyName("nuGolEquipeUm")]
        public int? GolsEquipeUm { get; set; }

        [JsonPropertyName("nuGolEquipeDois")]
        public int? GolsEquipeDois { get; set; }

        [JsonPropertyName("siglaUFUm")]
        public string? UFUm { get; set; }

        [JsonPropertyName("siglaUFDois")]
        public string? UFDois { get; set; }

        [JsonPropertyName("resultado")]
        public string? Resultado { get; set; }
    }
}
