using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Models;

namespace Loterias.Logging.Common.Services
{
    public class StructuredLogger : IStructuredLogger
    {
        private readonly string _graylogUrl;
        private readonly string _serviceName;
        private static readonly HttpClient _client = new();

        public StructuredLogger(string graylogUrl, string serviceName)
        {
            _graylogUrl = graylogUrl;
            _serviceName = serviceName;
        }

        public void Info(string message, object? details = null) =>
            SendLog("INFO", message, details);

        public void Warn(string message, object? details = null) =>
            SendLog("WARN", message, details);

        public void Error(string message, Exception ex, object? details = null) =>
            SendLog("ERROR", $"{message} | {ex.Message}", new { details, Stack = ex.StackTrace });

        private void SendLog(string level, string message, object? details)
        {
            try
            {
                var payload = new LogEntry
                {
                    Service = _serviceName,
                    Level = level,
                    Message = message,
                    Details = details
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _client.PostAsync(_graylogUrl, content).ConfigureAwait(false);
            }
            catch
            {
                // Evita loop de log; se Graylog falhar, ignora silenciosamente.
            }
        }
    }
}

