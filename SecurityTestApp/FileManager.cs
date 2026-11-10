using System;
using System.Collections.Generic; // Не забываем добавить
using System.IO;

namespace SecurityTestApp
{
    public static class FileManager
    {
        private static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");

        // Хранилища для защиты от брутфорса
        private static Dictionary<string, int> failedAttempts = new Dictionary<string, int>();
        private static Dictionary<string, DateTime> lockoutEnds = new Dictionary<string, DateTime>();

        // ... (методы InitializeFile и AddUser оставляем без изменений) ...

        public static string Authenticate(string username, string password)
        {
            // ПРОВЕРКА БЛОКИРОВКИ
            if (lockoutEnds.ContainsKey(username))
            {
                if (DateTime.Now < lockoutEnds[username])
                {
                    TimeSpan remaining = lockoutEnds[username] - DateTime.Now;
                    return $"LOCKED|{Math.Ceiling(remaining.TotalMinutes)}"; // Возвращаем спец. статус
                }
                else
                {
                    // Время вышло, снимаем блокировку
                    lockoutEnds.Remove(username);
                    failedAttempts[username] = 0;
                }
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 4 && parts[0] == username)
                {
                    string storedSalt = parts[1];
                    string storedHash = parts[2];
                    string role = parts[3];

                    string inputHash = SecurityHelper.ComputeHash(password, storedSalt);

                    if (inputHash == storedHash)
                    {
                        // Успех! Сбрасываем счетчик ошибок
                        failedAttempts[username] = 0;
                        return role;
                    }
                }
            }

            // НЕВЕРНЫЙ ПАРОЛЬ (или пользователь не найден)
            if (!failedAttempts.ContainsKey(username))
                failedAttempts[username] = 0;

            failedAttempts[username]++;

            if (failedAttempts[username] >= 3)
            {
                // Блокируем на 5 минут
                lockoutEnds[username] = DateTime.Now.AddMinutes(5);
                return "LOCKED|5";
            }

            return null;
        }
    }
}