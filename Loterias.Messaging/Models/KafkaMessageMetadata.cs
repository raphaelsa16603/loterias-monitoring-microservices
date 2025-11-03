using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json.Serialization;

namespace Loterias.Messaging.Models
{
    /// <summary>
    /// Representa os metadados comuns a todas as mensagens publicadas no Kafka.
    /// Usado para rastreamento e auditoria em todo o ecossistema de microsserviços.
    /// </summary>
    public class KafkaMessageMetadata
    {
        /// <summary>
        /// Identificador único da mensagem.
        /// </summary>
        [JsonPropertyName("messageId")]
        public Guid MessageId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Identificador de correlação (usado para agrupar eventos de um mesmo fluxo).
        /// </summary>
        [JsonPropertyName("correlationId")]
        public string CorrelationId { get; set; } = string.Empty;

        /// <summary>
        /// Serviço de origem (ex: CollectorDailyService, JobConsumerService).
        /// </summary>
        [JsonPropertyName("sourceService")]
        public string SourceService { get; set; } = string.Empty;

        /// <summary>
        /// Ambiente de execução (ex: development, qas, production).
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; set; } = "development";

        /// <summary>
        /// Data e hora UTC da publicação da mensagem.
        /// </summary>
        [JsonPropertyName("timestampUtc")]
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Tipo de evento (ex: SORTEIO_INSERIDO, LOG_EVENTO, JOB_EXECUTADO).
        /// </summary>
        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de rastreamento usado em observabilidade (Graylog, Graytrace, etc.).
        /// </summary>
        [JsonPropertyName("traceId")]
        public string TraceId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Hostname ou container de origem da mensagem.
        /// </summary>
        [JsonPropertyName("host")]
        public string Host { get; set; } = System.Environment.MachineName;

    }
}

