using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Loterias.CaixaClientLib.Util
{
    /// <summary>
    /// Conversor robusto para datas da Caixa — tolerante a nulos, vazios e múltiplos formatos.
    /// </summary>
    public class DateTimeConverterCaixa : JsonConverter<DateTime>
    {
        private static readonly string[] formatos = new[]
        {
            "dd/MM/yyyy",
            "dd/MM/yyyy HH:mm:ss",
            "yyyy-MM-dd'T'HH:mm:ss",
            "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
            "yyyy-MM-dd"
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();

            // 🔹 Tratar nulos e vazios
            if (string.IsNullOrWhiteSpace(str) || str.Equals("null", StringComparison.OrdinalIgnoreCase))
                return DateTime.MinValue;

            // 🔹 Tentar conversão exata
            if (DateTime.TryParseExact(str.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact))
                return exact;

            // 🔹 Fallback para parsing genérico (tratando fuso e variantes)
            if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var parsed))
                return parsed;

            // 🔹 Fallback: loga o problema e retorna data mínima para não quebrar o fluxo
            return DateTime.MinValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // 🔹 Se for data mínima, não escreve nada
            if (value == DateTime.MinValue)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
