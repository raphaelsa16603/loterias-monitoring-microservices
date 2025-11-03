using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loterias.Messaging.Interfaces
{
    public interface IMessageProducer
    {
        Task PublishAsync<T>(string topic, T message, CancellationToken ct = default);
    }
}

