using System;
using System.IO;

namespace SecurityTestApp
{
    public static class FileManager
    {
        private static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");

        public static void InitializeFile()
        {
            if (!File.Exists(filePath))
            {
                // Для админа тоже генерируем соль и хеш для пароля "123456"
                string salt = SecurityHelper.GenerateSalt();
                string hash = SecurityHelper.ComputeHash("123456", salt);
                File.WriteAllText(filePath, $"admin|{salt}|{hash}|Admin\n");
            }
        }

        public static bool AddUser(string username, string password, string role = "User")
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (line.Split('|')[0] == username) return false;
            }

            // Генерируем соль и хеш для нового пользователя
            string salt = SecurityHelper.GenerateSalt();
            string hash = SecurityHelper.ComputeHash(password, salt);

            File.AppendAllText(filePath, $"{username}|{salt}|{hash}|{role}\n");
            return true;
        }

        public static string Authenticate(string username, string password)
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 4 && parts[0] == username)
                {
                    string storedSalt = parts[1];
                    string storedHash = parts[2];
                    string role = parts[3];

                    // Хешируем введенный пароль с той же солью и сравниваем
                    string inputHash = SecurityHelper.ComputeHash(password, storedSalt);

                    if (inputHash == storedHash)
                    {
                        return role;
                    }
                }
            }
            return null;
        }
    }
}