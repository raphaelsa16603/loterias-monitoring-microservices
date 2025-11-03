using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Loterias.CaixaClientLib.Models
{
    public class ResultadoSorteioCaixa
    {
        public string TipoJogo { get; set; } = string.Empty;
        public int NumeroConcurso { get; set; }
        public DateTime DataApuracao { get; set; }
        public List<int> Dezenas { get; set; } = new();
        public List<PremiacaoCaixa> Premiacao { get; set; } = new();
        public decimal ValorArrecadado { get; set; }
        public bool Acumulado { get; set; }
        public decimal ValorAcumuladoProximoConcurso { get; set; }
        public string LocalSorteio { get; set; } = string.Empty;
        public string MunicipioUF { get; set; } = string.Empty;

        public static ResultadoSorteioCaixa FromResponse(CaixaResponse response)
        {
            return new ResultadoSorteioCaixa
            {
                TipoJogo = response.TipoJogo,
                NumeroConcurso = response.NumeroConcurso,
                DataApuracao = response.DataApuracao,
                Dezenas = response.Dezenas.Select(d => int.Parse(d)).ToList(),
                Premiacao = response.Premiacao,
                ValorArrecadado = response.ValorArrecadado,
                Acumulado = response.Acumulado,
                ValorAcumuladoProximoConcurso = response.ValorAcumuladoProximoConcurso,
                LocalSorteio = response.LocalSorteio,
                MunicipioUF = response.NomeMunicipioUFSorteio
            };
        }
    }
}

