using Loterias.CaixaClientLib.Util;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loterias.CaixaClientLib.Models
{
    public class CaixaResponse
    {
        [JsonPropertyName("tipoJogo")]
        public string TipoJogo { get; set; } = string.Empty;

        [JsonPropertyName("numero")]
        public int NumeroConcurso { get; set; }

        [JsonPropertyName("dataApuracao")]
        [JsonConverter(typeof(DateTimeConverterCaixa))]
        public DateTime DataApuracao { get; set; }

        [JsonPropertyName("dataProximoConcurso")]
        [JsonConverter(typeof(DateTimeConverterCaixa))]
        public DateTime? DataProximoConcurso { get; set; }

        [JsonPropertyName("numeroConcursoProximo")]
        public int? NumeroConcursoProximo { get; set; }

        [JsonPropertyName("numeroConcursoAnterior")]
        public int? NumeroConcursoAnterior { get; set; }

        [JsonPropertyName("numeroConcursoFinal_0_5")]
        public int? NumeroConcursoFinal05 { get; set; }

        [JsonPropertyName("ultimoConcurso")]
        public bool? UltimoConcurso { get; set; }

        [JsonPropertyName("indicadorConcursoEspecial")]
        public int? IndicadorConcursoEspecial { get; set; }

        [JsonPropertyName("acumulado")]
        public bool Acumulado { get; set; }

        [JsonPropertyName("observacao")]
        public string? Observacao { get; set; }

        [JsonPropertyName("localSorteio")]
        public string LocalSorteio { get; set; } = string.Empty;

        [JsonPropertyName("nomeMunicipioUFSorteio")]
        public string NomeMunicipioUFSorteio { get; set; } = string.Empty;

        // 🔹 Dezenas e sorteios
        [JsonPropertyName("dezenasSorteadasOrdemSorteio")]
        public List<string> DezenasSorteadasOrdemSorteio { get; set; } = new();

        [JsonPropertyName("listaDezenas")]
        public List<string> ListaDezenas { get; set; } = new();

        [JsonPropertyName("listaDezenasSegundoSorteio")]
        public List<string>? ListaDezenasSegundoSorteio { get; set; }

        [JsonPropertyName("trevosSorteados")]
        public List<string>? TrevosSorteados { get; set; }

        // 🔹 Resultados esportivos (Loteca)
        [JsonPropertyName("listaResultadoEquipeEsportiva")]
        public List<ResultadoEquipeEsportiva>? ListaResultadoEquipeEsportiva { get; set; }

        // 🔹 Municípios premiados (Federal, Lotofácil, Lotomania, Loteca)
        [JsonPropertyName("listaMunicipioUFGanhadores")]
        public List<MunicipioUFGanhador>? ListaMunicipioUFGanhadores { get; set; }

        // 🔹 Premiações
        [JsonPropertyName("listaRateioPremio")]
        public List<PremiacaoCaixa> Premiacao { get; set; } = new();

        // 🔹 Valores financeiros
        [JsonPropertyName("valorArrecadado")]
        public decimal ValorArrecadado { get; set; }

        [JsonPropertyName("valorAcumuladoConcurso_0_5")]
        public decimal? ValorAcumuladoConcurso05 { get; set; }

        [JsonPropertyName("valorAcumuladoConcursoEspecial")]
        public decimal? ValorAcumuladoConcursoEspecial { get; set; }

        [JsonPropertyName("valorAcumuladoProximoConcurso")]
        public decimal? ValorAcumuladoProximoConcurso { get; set; }

        [JsonPropertyName("valorEstimadoProximoConcurso")]
        public decimal? ValorEstimadoProximoConcurso { get; set; }

        [JsonPropertyName("valorSaldoReservaGarantidora")]
        public decimal? ValorSaldoReservaGarantidora { get; set; }

        [JsonPropertyName("valorTotalPremioFaixaUm")]
        public decimal? ValorTotalPremioFaixaUm { get; set; }

        // 🔹 Campos extras de identificação
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("numeroJogo")]
        public int? NumeroJogo { get; set; }

        [JsonPropertyName("nomeTimeCoracaoMesSorte")]
        public string? NomeTimeCoracaoMesSorte { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object>? CamposExtras { get; set; }
    }
}
