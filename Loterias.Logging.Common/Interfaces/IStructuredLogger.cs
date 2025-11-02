using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loterias.Logging.Common.Interfaces
{
    public interface IStructuredLogger
    {
        void Info(string message, object? details = null);
        void Warn(string message, object? details = null);
        void Error(string message, Exception ex, object? details = null);
    }
}
