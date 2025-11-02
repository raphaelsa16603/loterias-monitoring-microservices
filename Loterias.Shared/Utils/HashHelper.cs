using System.Security.Cryptography;
using System.Text;

namespace Loterias.Shared.Utils
{
    public static class HashHelper
    {
        public static string ComputeHash(string input)
        {
            try
            {
                using var sha = SHA256.Create();
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HashHelper] Erro ao gerar hash: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
