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

        // 🔹 Identificação principal
        [BsonElement("tipoLoteria")]
        public string TipoLoteria { get; set; } = string.Empty;

        [BsonElement("concurso")]
        public int Concurso { get; set; }

        [BsonElement("dataSorteio")]
        public DateTime DataSorteio { get; set; }

        [BsonElement("localSorteio")]
        public string LocalSorteio { get; set; } = string.Empty;

        // 🔹 Dados sorteados
        [BsonElement("numerosSorteados")]
        public List<string> NumerosSorteados { get; set; } = new();

        [BsonElement("dezenasEmOrdem")]
        public List<string> DezenasEmOrdem { get; set; } = new();

        [BsonElement("dezenasSegundoSorteio")]
        public List<string> DezenasSegundoSorteio { get; set; } = new();

        [BsonElement("trevosSorteados")]
        public List<string> TrevosSorteados { get; set; } = new();

        [BsonElement("nomeTimeCoracaoMesSorte")]
        public string NomeTimeCoracaoMesSorte { get; set; } = string.Empty;

        // 🔹 Valores
        public decimal ArrecadacaoTotal { get; set; }
        public bool Acumulado { get; set; }
        public decimal ValorAcumuladoProxConcurso { get; set; }
        public decimal ValorEstimadoProximoConcurso { get; set; }

        // 🔹 Premiações e extras
        public List<Premiacao> Premiacoes { get; set; } = new();

        public string Observacao { get; set; } = string.Empty;

        // 🔹 Metadados internos
        public string FonteDados { get; set; } = "Caixa";
        public DateTime DataColeta { get; set; } = DateTime.UtcNow;
        public DateTime DataProcessamento { get; set; } = DateTime.UtcNow;

        [BsonIgnoreIfNull]
        public string JsonCompleto { get; set; } = string.Empty;
    }
}
