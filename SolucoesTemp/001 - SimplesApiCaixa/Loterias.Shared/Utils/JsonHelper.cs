using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Loterias.Shared.Utils
{
    public static class JsonHelper
    {
        public static string Serialize(object obj, ILogger? logger = null)
        {
            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "[JsonHelper] Erro ao serializar objeto: {@Obj}", obj);
                return string.Empty;
            }
        }

        public static T? Deserialize<T>(string json, ILogger? logger = null)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "[JsonHelper] Erro ao desserializar JSON: {Json}", json);
                return default;
            }
        }
    }
}

