using System.Text.Json;
using Loterias.Logging.Common.Interfaces;

namespace Loterias.Shared.Utils
{
    public static class JsonHelper
    {
        public static string Serialize(object obj, IStructuredLogger? logger = null)
        {
            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
            }
            catch (Exception ex)
            {
                logger?.Error("[JsonHelper] Erro ao serializar objeto", ex, obj);
                return string.Empty;
            }
        }

        public static T? Deserialize<T>(string json, IStructuredLogger? logger = null)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                logger?.Error("[JsonHelper] Erro ao desserializar JSON", ex, json);
                return default;
            }
        }
    }
}

