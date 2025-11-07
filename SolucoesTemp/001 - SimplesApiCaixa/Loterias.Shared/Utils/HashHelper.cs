using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Loterias.Shared.Utils
{
    public static class HashHelper
    {
        public static string ComputeHash(string input, ILogger? logger = null)
        {
            try
            {
                using var sha = SHA256.Create();
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "[HashHelper] Erro ao gerar hash. Input: {Input}", input);
                return string.Empty;
            }
        }
    }
}
