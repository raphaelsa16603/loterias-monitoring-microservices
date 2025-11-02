using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Loterias.Shared.Models
{
    public class Premiacao
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string TipoLoteria { get; set; } = string.Empty;
        public int Concurso { get; set; }
        public int Faixa { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Ganhadores { get; set; }
        public decimal ValorPremio { get; set; }
        public DateTime DataSorteio { get; set; }
    }
}

