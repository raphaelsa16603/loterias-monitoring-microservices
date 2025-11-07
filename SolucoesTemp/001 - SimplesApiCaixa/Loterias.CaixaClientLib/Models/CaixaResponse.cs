using Loterias.CaixaClientLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        [JsonPropertyName("dezenasSorteadasOrdemSorteio")]
        public List<string> Dezenas { get; set; } = new();

        [JsonPropertyName("listaRateioPremio")]
        public List<PremiacaoCaixa> Premiacao { get; set; } = new();

        [JsonPropertyName("valorArrecadado")]
        public decimal ValorArrecadado { get; set; }

        [JsonPropertyName("valorAcumuladoProximoConcurso")]
        public decimal ValorAcumuladoProximoConcurso { get; set; }

        [JsonPropertyName("localSorteio")]
        public string LocalSorteio { get; set; } = string.Empty;

        [JsonPropertyName("nomeMunicipioUFSorteio")]
        public string NomeMunicipioUFSorteio { get; set; } = string.Empty;

        [JsonPropertyName("dataProximoConcurso")]
        [JsonConverter(typeof(DateTimeConverterCaixa))]
        public DateTime? DataProximoConcurso { get; set; }

        [JsonPropertyName("numeroConcursoProximo")]
        public int? NumeroConcursoProximo { get; set; }

        [JsonPropertyName("observacao")]
        public string? Observacao { get; set; }

        [JsonPropertyName("acumulado")]
        public bool Acumulado { get; set; }
    }
}

