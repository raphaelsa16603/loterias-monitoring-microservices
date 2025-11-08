using System.Text.Json.Serialization;

namespace Loterias.CaixaApiService.Models
{
    public sealed class ResultadoEquipeEsportivaDto
    {
        [JsonPropertyName("diaSemana")]
        public string? DiaSemana { get; set; }

        [JsonPropertyName("dtJogo")]
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
    }
}
