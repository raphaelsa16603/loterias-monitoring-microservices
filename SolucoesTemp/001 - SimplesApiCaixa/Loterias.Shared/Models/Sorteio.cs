using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Loterias.Shared.Models
{
    public class Sorteio
    {
        [BsonId]
        public ObjectId Id { get; set; }

        // 🔹 Identificadores básicos e versão da coleta
        public int NumeroJogo { get; set; }
        public DateTime DataJogo { get; set; }
        public string TipoJogo { get; set; } = string.Empty;
        public string JsonCompleto { get; set; } = string.Empty;

        [BsonIgnoreIfNull] public string VersaoApiCaixa { get; set; } = string.Empty;
        [BsonIgnoreIfNull] public string UrlFonteCaixa { get; set; } = string.Empty;
        [BsonIgnoreIfNull] public DateTime? DataConsultaApi { get; set; } = DateTime.UtcNow;

        // 🔹 Dados estruturados do sorteio
        public string TipoLoteria { get; set; } = string.Empty;
        public int Concurso { get; set; }
        public DateTime DataSorteio { get; set; }
        public List<int> NumerosSorteados { get; set; } = new();
        public string LocalSorteio { get; set; } = string.Empty;
        public List<Premiacao> Premiacoes { get; set; } = new();
        public decimal ArrecadacaoTotal { get; set; }
        public bool Acumulado { get; set; }
        public decimal ValorAcumuladoProxConcurso { get; set; }
        public string FonteDados { get; set; } = string.Empty;
        public DateTime DataColeta { get; set; } = DateTime.UtcNow;
        public DateTime DataProcessamento { get; set; } = DateTime.UtcNow;
        public string HashIntegridade { get; set; } = string.Empty;
    }
}
