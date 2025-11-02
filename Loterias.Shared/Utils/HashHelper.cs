using System.Security.Cryptography;
using System.Text;
using Loterias.Logging.Common.Interfaces;

namespace Loterias.Shared.Utils
{
    public static class HashHelper
    {
        public static string ComputeHash(string input, IStructuredLogger? logger = null)
        {
            try
            {
                using var sha = SHA256.Create();
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception ex)
            {
                logger?.Error("[HashHelper] Erro ao gerar hash", ex, new { Input = input });
                return string.Empty;
            }
        }
    }
}
