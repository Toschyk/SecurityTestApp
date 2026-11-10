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
                // Создаем тестового пользователя
                File.WriteAllText(filePath, "admin|123456|Admin\n");
            }
        }

        public static bool AddUser(string username, string password, string role = "User")
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (line.Split('|')[0] == username) return false;
            }
            // УЯЗВИМОСТЬ №1: Пароль сохраняется в открытом виде!
            File.AppendAllText(filePath, $"{username}|{password}|{role}\n");
            return true;
        }

        public static string Authenticate(string username, string password)
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 3 && parts[0] == username && parts[1] == password)
                {
                    return parts[2];
                }
            }
            return null;
        }
    }
}