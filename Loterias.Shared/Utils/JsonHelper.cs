using System.Text.Json;

namespace Loterias.Shared.Utils
{
    public static class JsonHelper
    {
        public static string Serialize(object obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JsonHelper] Erro ao serializar objeto: {ex.Message}");
                return string.Empty;
            }
        }

        public static T? Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JsonHelper] Erro ao desserializar JSON: {ex.Message}");
                return default;
            }
        }
    }
}
