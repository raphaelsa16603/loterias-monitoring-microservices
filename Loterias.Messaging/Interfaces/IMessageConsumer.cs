using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loterias.Messaging.Interfaces
{
    public interface IMessageConsumer
    {
        Task ConsumeAsync(string topic, Func<string, Task> handler, CancellationToken ct = default);
    }
}

