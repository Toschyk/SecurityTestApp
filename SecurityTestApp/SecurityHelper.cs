using System;
using System.Security.Cryptography;
using System.Text;

namespace SecurityTestApp
{
    public static class SecurityHelper
    {
        // Генерация случайной соли (16 байт)
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Вычисление хеша (Пароль + Соль)
        public static string ComputeHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}