using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Loterias.Logging.Common.Interfaces;
using Loterias.Logging.Common.Services;

namespace Loterias.Logging.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Opções de configuração do logger estruturado apontando para o Graylog.
        /// </summary>
        public sealed class LoggingOptions
        {
            /// <summary>
            /// URL do endpoint HTTP do Graylog (ex.: http://graylog:12201/gelf)
            /// </summary>
            public string? GraylogUrl { get; set; }

            /// <summary>
            /// Nome do serviço (aparece no Graylog).
            /// </summary>
            public string? ServiceName { get; set; }
        }

        /// <summary>
        /// Registra o IStructuredLogger lendo de IConfiguration.
        /// Por padrão usa a seção "Logging:Graylog" e/ou as variáveis de ambiente GRAYLOG_URL e SERVICE_NAME.
        /// </summary>
        public static IServiceCollection AddStructuredLogger(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionPath = "Logging:Graylog")
        {
            var section = configuration.GetSection(sectionPath);

            var options = new LoggingOptions
            {
                GraylogUrl = section["GraylogUrl"]
                             ?? Environment.GetEnvironmentVariable("GRAYLOG_URL"),
                ServiceName = section["ServiceName"]
                              ?? Environment.GetEnvironmentVariable("SERVICE_NAME")
            };

            return services.AddStructuredLogger(options);
        }

        /// <summary>
        /// Registra o IStructuredLogger recebendo as opções via delegate.
        /// </summary>
        public static IServiceCollection AddStructuredLogger(
            this IServiceCollection services,
            Action<LoggingOptions> configure)
        {
            var opts = new LoggingOptions();
            configure?.Invoke(opts);
            return services.AddStructuredLogger(opts);
        }

        /// <summary>
        /// Registra o IStructuredLogger recebendo as opções já materializadas.
        /// </summary>
        public static IServiceCollection AddStructuredLogger(
            this IServiceCollection services,
            LoggingOptions options)
        {
            // Validação mínima: se faltar GraylogUrl ou ServiceName, usa NoOp (sem console).
            if (string.IsNullOrWhiteSpace(options?.GraylogUrl) ||
                string.IsNullOrWhiteSpace(options?.ServiceName))
            {
                services.TryAddSingleton<IStructuredLogger, NoOpStructuredLogger>();
                return services;
            }

            services.TryAddSingleton<IStructuredLogger>(
                _ =>
                {
                    // Extrai host e porta da URL do Graylog
                    var uri = new Uri(options!.GraylogUrl!);
                    var host = uri.Host;
                    var port = uri.Port;
                    return new StructuredLogger(host, port, options!.ServiceName!);
                }
            );

            return services;
        }

        /// <summary>
        /// Logger nulo (sem saída). Usado quando não há Graylog configurado.
        /// Mantém o Shared sem Console.WriteLine e sem dependências externas.
        /// </summary>
        private sealed class NoOpStructuredLogger : IStructuredLogger
        {
            public void Info(string message, object? details = null) { /* no-op */ }
            public void Warn(string message, object? details = null) { /* no-op */ }
            public void Error(string message, Exception ex, object? details = null) { /* no-op */ }
        }
    }
}

