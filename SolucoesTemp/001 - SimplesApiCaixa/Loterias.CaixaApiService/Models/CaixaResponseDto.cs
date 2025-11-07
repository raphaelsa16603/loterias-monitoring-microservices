using System.Text.Json.Serialization;

namespace Loterias.CaixaApiService.Models
{
    /// <summary>
    /// DTO “bruto” que representa a resposta da API pública da Caixa.
    /// Ele é usado somente dentro do CaixaApiService (proxy + cache).
    /// A normalização para o modelo interno (Loterias.Shared) fica a cargo
    /// da camada de aplicação/mapper quando necessário.
    /// </summary>
    public sealed class CaixaResponseDto
    {
        /// <summary>Tipo da loteria (ex.: "megasena", "quina", "lotofacil").</summary>
        [JsonPropertyName("tipoLoteria")]
        public string? TipoLoteria { get; set; }

        /// <summary>Número do concurso.</summary>
        [JsonPropertyName("numero")]
        public int? Concurso { get; set; }

        /// <summary>Data/hora oficial do sorteio (UTC quando houver).</summary>
        [JsonPropertyName("dataApuracao")]
        public DateTime? DataSorteio { get; set; }

        /// <summary>Local do sorteio (quando fornecido pela API).</summary>
        [JsonPropertyName("localSorteio")]
        public string? LocalSorteio { get; set; }

        /// <summary>
        /// Dezenas sorteadas no formato de string. A API da Caixa costuma devolver
        /// como "listaDezenas" (array de strings) ou "dezenasSorteadasOrdemSorteio".
        /// Aqui normalizamos para int[] quando possível.
        /// </summary>
        [JsonPropertyName("listaDezenas")]
        public List<string>? ListaDezenas { get; set; }

        /// <summary>
        /// Algumas modalidades trazem também “dezenas em ordem de sorteio”.
        /// Mantemos para referência. Se vier preenchido, ele será usado
        /// primeiro na normalização de <see cref="NumerosSorteados"/>.
        /// </summary>
        [JsonPropertyName("dezenasSorteadasOrdemSorteio")]
        public List<string>? DezenasEmOrdem { get; set; }

        /// <summary>Valor total arrecadado no concurso.</summary>
        [JsonPropertyName("valorArrecadado")]
        public decimal? ArrecadacaoTotal { get; set; }

        /// <summary>Indica se o prêmio principal ficou acumulado.</summary>
        [JsonPropertyName("acumulado")]
        public bool? Acumulado { get; set; }

        /// <summary>Valor estimado/acumulado para o próximo concurso.</summary>
        [JsonPropertyName("valorAcumuladoProximoConcurso")]
        public decimal? ValorAcumuladoProxConcurso { get; set; }

        /// <summary>Premiações por faixa.</summary>
        [JsonPropertyName("listaRateioPremio")]
        public List<PremiacaoDto>? Premiacoes { get; set; }

        /// <summary>
        /// Normaliza as dezenas para inteiros, priorizando a lista em ordem de sorteio quando existir.
        /// </summary>
        [JsonIgnore]
        public int[] NumerosSorteados
        {
            get
            {
                var fonte = (DezenasEmOrdem?.Count > 0 ? DezenasEmOrdem : ListaDezenas) ?? new List<string>();
                return fonte
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => int.TryParse(s, out var n) ? n : -1)
                    .Where(n => n >= 0)
                    .ToArray();
            }
        }

        [JsonPropertyName("nomeMunicipioUFSorteio")]
        public string NomeMunicipioUFSorteio { get; set; } = string.Empty;

        [JsonPropertyName("dataProximoConcurso")]
        public DateTime? DataProximoConcurso { get; set; }

        [JsonPropertyName("numeroConcursoProximo")]
        public int? NumeroConcursoProximo { get; set; }

        [JsonPropertyName("observacao")]
        public string? Observacao { get; set; }

        /// <summary>
        /// Retorna um identificador de correlação útil para logs (ex.: "megasena-2663").
        /// </summary>
        [JsonIgnore]
        public string CorrelationId => $"{TipoLoteria ?? "loteria"}-{Concurso?.ToString() ?? "s/conc"}";
    }


}
