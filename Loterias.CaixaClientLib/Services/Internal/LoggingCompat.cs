using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Loterias.CaixaClientLib.Services.Internal
{
    internal static class LoggingCompat
    {
        /// <summary>
        /// Tenta logar usando várias assinaturas comuns em libs de logging estruturado.
        /// Se nada bater, faz fallback no ILogger padrão.
        /// Assinaturas tentadas (por reflexão), nesta ordem:
        ///   - LogAsync(string level, string message, object? data = null, Exception? ex = null)
        ///   - Log(string level, string message, object? data = null, Exception? ex = null)
        ///   - WriteAsync(string level, string message, object? data = null, Exception? ex = null)
        ///   - Write(string level, string message, object? data = null, Exception? ex = null)
        ///   - LogAsync(object logEvent)
        ///   - Log(object logEvent)
        /// </summary>
        public static async Task SafeLogAsync(
            object? structuredLogger,
            ILogger? fallbackLogger,
            string level,
            string message,
            object? data = null,
            Exception? exception = null)
        {
            // 1) Tentativa via métodos de assinatura (level, message, data, exception)
            if (structuredLogger != null)
            {
                var t = structuredLogger.GetType();

                var candidates = new[]
                {
                    "LogAsync", "Log",
                    "WriteAsync", "Write"
                };

                foreach (var name in candidates)
                {
                    var m = FindMethod(t, name, new[] { typeof(string), typeof(string), typeof(object), typeof(Exception) })
                         ?? FindMethod(t, name, new[] { typeof(string), typeof(string), typeof(object) })
                         ?? FindMethod(t, name, new[] { typeof(string), typeof(string) });

                    if (m != null)
                    {
                        var args = BuildArgsFor(m, level, message, data, exception);
                        var result = m.Invoke(structuredLogger, args);

                        if (result is Task task)
                        {
                            await task.ConfigureAwait(false);
                        }
                        return;
                    }
                }

                // 2) Tentativa via evento estruturado único: LogAsync(object evt)/Log(object evt)
                var evtMethod =
                    FindMethod(t, "LogAsync", new[] { typeof(object) }) ??
                    FindMethod(t, "Log", new[] { typeof(object) }) ??
                    FindMethod(t, "WriteAsync", new[] { typeof(object) }) ??
                    FindMethod(t, "Write", new[] { typeof(object) });

                if (evtMethod != null)
                {
                    var evt = new
                    {
                        Service = "Loterias.CaixaClientLib",
                        Level = level,
                        Message = message,
                        Data = data,
                        ExceptionMessage = exception?.Message,
                        ExceptionStack = exception?.StackTrace,
                        Timestamp = DateTimeOffset.UtcNow
                    };

                    var result = evtMethod.Invoke(structuredLogger, new[] { (object)evt });
                    if (result is Task task) await task.ConfigureAwait(false);
                    return;
                }
            }

            // 3) Fallback no ILogger
            if (fallbackLogger != null)
            {
                var payload = data is null ? "" : $" | data: {System.Text.Json.JsonSerializer.Serialize(data)}";

                switch (level.ToUpperInvariant())
                {
                    case "TRACE": fallbackLogger.LogTrace(exception, "{Message}{Payload}", message, payload); break;
                    case "DEBUG": fallbackLogger.LogDebug(exception, "{Message}{Payload}", message, payload); break;
                    case "INFO": fallbackLogger.LogInformation(exception, "{Message}{Payload}", message, payload); break;
                    case "WARN":
                    case "WARNING": fallbackLogger.LogWarning(exception, "{Message}{Payload}", message, payload); break;
                    case "ERROR": fallbackLogger.LogError(exception, "{Message}{Payload}", message, payload); break;
                    case "CRITICAL":
                    case "FATAL": fallbackLogger.LogCritical(exception, "{Message}{Payload}", message, payload); break;
                    default: fallbackLogger.LogInformation(exception, "{Message}{Payload}", message, payload); break;
                }
            }
        }

        private static MethodInfo? FindMethod(Type t, string name, Type[] paramTypes)
            => t.GetMethod(name, BindingFlags.Public | BindingFlags.Instance, binder: null, types: paramTypes, modifiers: null);

        private static object[] BuildArgsFor(MethodInfo m, string level, string message, object? data, Exception? ex)
        {
            var ps = m.GetParameters();
            var args = new object?[ps.Length];

            for (int i = 0; i < ps.Length; i++)
            {
                var pt = ps[i].ParameterType;

                if (pt == typeof(string) && (ps[i].Name?.Equals("level", StringComparison.OrdinalIgnoreCase) ?? false))
                    args[i] = level;
                else if (pt == typeof(string) && (ps[i].Name?.Equals("message", StringComparison.OrdinalIgnoreCase) ?? false))
                    args[i] = message;
                else if (pt == typeof(object) && (ps[i].Name?.Equals("data", StringComparison.OrdinalIgnoreCase) ?? false))
                    args[i] = data!;
                else if (typeof(Exception).IsAssignableFrom(pt))
                    args[i] = ex!;
                else
                    args[i] = GetDefault(pt);
            }

            return args!;
        }

        private static object? GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}

