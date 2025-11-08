using System;
using System.Collections.Generic;
using System.Linq;

namespace Loterias.CaixaClientLib.Models
{
    public class ResultadoSorteioCaixa
    {
        public string TipoJogo { get; set; } = string.Empty;
        public int NumeroConcurso { get; set; }
        public DateTime DataApuracao { get; set; }
        public List<string> Dezenas { get; set; } = new();
        public List<string>? DezenasSegundoSorteio { get; set; }
        public List<string>? TrevosSorteados { get; set; }
        public string? NomeTimeCoracaoMesSorte { get; set; }
        public List<PremiacaoCaixa> Premiacao { get; set; } = new();
        public decimal ValorArrecadado { get; set; }
        public bool Acumulado { get; set; }
        public decimal? ValorAcumuladoProximoConcurso { get; set; }
        public string LocalSorteio { get; set; } = string.Empty;
        public string MunicipioUF { get; set; } = string.Empty;

        // 🔹 Construtor de conversão da API
        public static ResultadoSorteioCaixa FromResponse(CaixaResponse response)
        {
            var dezenas = response.ListaDezenas?.Any() == true
                ? response.ListaDezenas
                : response.DezenasSorteadasOrdemSorteio;

            return new ResultadoSorteioCaixa
            {
                TipoJogo = response.TipoJogo,
                NumeroConcurso = response.NumeroConcurso,
                DataApuracao = response.DataApuracao,
                Dezenas = dezenas ?? new List<string>(),
                DezenasSegundoSorteio = response.ListaDezenasSegundoSorteio,
                TrevosSorteados = response.TrevosSorteados,
                NomeTimeCoracaoMesSorte = response.NomeTimeCoracaoMesSorte,
                Premiacao = response.Premiacao ?? new List<PremiacaoCaixa>(),
                ValorArrecadado = response.ValorArrecadado,
                Acumulado = response.Acumulado,
                ValorAcumuladoProximoConcurso = response.ValorAcumuladoProximoConcurso,
                LocalSorteio = response.LocalSorteio,
                MunicipioUF = response.NomeMunicipioUFSorteio
            };
        }
    }
}
