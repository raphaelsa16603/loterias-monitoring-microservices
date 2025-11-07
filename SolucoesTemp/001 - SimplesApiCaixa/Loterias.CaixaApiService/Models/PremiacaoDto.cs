using System.Text.Json.Serialization;

namespace Loterias.CaixaApiService.Models
{

    /// <summary>Faixa de premiação retornada pela API da Caixa.</summary>
    public sealed class PremiacaoDto
    {
        /// <summary>Descrição da faixa (ex.: "6 acertos").</summary>
        [JsonPropertyName("descricaoFaixa")]
        public string? Descricao { get; set; }

        /// <summary>Número da faixa (1, 2, 3...).</summary>
        [JsonPropertyName("faixa")]
        public int? Faixa { get; set; }

        /// <summary>Quantidade de ganhadores na faixa.</summary>
        [JsonPropertyName("numeroDeGanhadores")]
        public int? Ganhadores { get; set; }

        /// <summary>Valor do prêmio por ganhador na faixa.</summary>
        [JsonPropertyName("valorPremio")]
        public decimal? ValorPremio { get; set; }
    }
}
