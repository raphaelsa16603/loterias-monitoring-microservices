using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Models;

namespace Loterias.Logging.Common.Services
{
    public class StructuredLogger : IStructuredLogger
    {
        private readonly string _graylogHost;
        private readonly int _graylogPort;
        private readonly string _serviceName;
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _graylogEndpoint;

        public StructuredLogger(string graylogHost, int graylogPort, string serviceName)
        {
            _graylogHost = graylogHost;
            _graylogPort = graylogPort;
            _serviceName = serviceName;
            _udpClient = new UdpClient();
            _graylogEndpoint = new IPEndPoint(Dns.GetHostAddresses(graylogHost)[0], graylogPort);
        }

        public void Info(string message, object? details = null)
            => SendLog("6", message, details); // 6 = Informational

        public void Warn(string message, object? details = null)
            => SendLog("4", message, details); // 4 = Warning

        public void Error(string message, Exception ex, object? details = null)
            => SendLog("3", $"{message} | {ex.Message}", new { details, ex.StackTrace }); // 3 = Error

        private void SendLog(string level, string message, object? details)
        {
            try
            {
                var log = new
                {
                    version = "1.1",
                    host = _serviceName,
                    short_message = message,
                    level = level,
                    _details = details,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                var json = JsonSerializer.Serialize(log);
                var bytes = Encoding.UTF8.GetBytes(json);

                _udpClient.Send(bytes, bytes.Length, _graylogEndpoint);
            }
            catch
            {
                // Evita loop infinito se Graylog estiver offline.
            }
        }
    }
}
