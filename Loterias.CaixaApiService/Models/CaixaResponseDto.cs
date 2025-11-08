using System.Text.Json.Serialization;

namespace Loterias.CaixaApiService.Models
{
    public sealed class CaixaResponseDto
    {
        [JsonPropertyName("tipoLoteria")]
        public string? TipoLoteria { get; set; }

        [JsonPropertyName("numero")]
        public int? Concurso { get; set; }

        [JsonPropertyName("dataApuracao")]
        public DateTime? DataSorteio { get; set; }

        [JsonPropertyName("localSorteio")]
        public string? LocalSorteio { get; set; }

        [JsonPropertyName("nomeMunicipioUFSorteio")]
        public string? NomeMunicipioUFSorteio { get; set; }

        [JsonPropertyName("listaDezenas")]
        public List<string>? ListaDezenas { get; set; }

        [JsonPropertyName("dezenasSorteadasOrdemSorteio")]
        public List<string>? DezenasEmOrdem { get; set; }

        [JsonPropertyName("listaDezenasSegundoSorteio")]
        public List<string>? DezenasSegundoSorteio { get; set; }

        [JsonPropertyName("trevosSorteados")]
        public List<string>? TrevosSorteados { get; set; }

        [JsonPropertyName("nomeTimeCoracaoMesSorte")]
        public string? NomeTimeCoracaoMesSorte { get; set; }

        [JsonPropertyName("valorArrecadado")]
        public decimal? ArrecadacaoTotal { get; set; }

        [JsonPropertyName("acumulado")]
        public bool? Acumulado { get; set; }

        [JsonPropertyName("valorAcumuladoProximoConcurso")]
        public decimal? ValorAcumuladoProxConcurso { get; set; }

        [JsonPropertyName("valorAcumuladoConcursoEspecial")]
        public decimal? ValorAcumuladoConcursoEspecial { get; set; }

        [JsonPropertyName("valorEstimadoProximoConcurso")]
        public decimal? ValorEstimadoProximoConcurso { get; set; }

        [JsonPropertyName("valorSaldoReservaGarantidora")]
        public decimal? ValorSaldoReservaGarantidora { get; set; }

        [JsonPropertyName("valorTotalPremioFaixaUm")]
        public decimal? ValorTotalPremioFaixaUm { get; set; }

        [JsonPropertyName("premiacoes")]
        public List<PremiacaoDto>? Premiacoes { get; set; }

        [JsonPropertyName("listaMunicipioUFGanhadores")]
        public List<MunicipioUFGanhadorDto>? MunicipiosGanhadores { get; set; }

        [JsonPropertyName("listaResultadoEquipeEsportiva")]
        public List<ResultadoEquipeEsportivaDto>? ResultadosEsportivos { get; set; }

        [JsonPropertyName("dataProximoConcurso")]
        public DateTime? DataProximoConcurso { get; set; }

        [JsonPropertyName("numeroConcursoProximo")]
        public int? NumeroConcursoProximo { get; set; }

        [JsonPropertyName("observacao")]
        public string? Observacao { get; set; }

        [JsonIgnore]
        public string CorrelationId => $"{TipoLoteria ?? "loteria"}-{Concurso?.ToString() ?? "s/conc"}";
    }
}
