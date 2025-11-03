using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loterias.Messaging.Kafka
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = "localhost:9092";
        public string GroupId { get; set; } = "loterias-consumers";
        public bool EnableAutoCommit { get; set; } = true;
        public int AutoOffsetReset { get; set; } = 1; // earliest
        public string SecurityProtocol { get; set; } = "PLAINTEXT";
    }
}

