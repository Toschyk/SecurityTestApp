using System;
using System.Collections.Generic;
using System.IO;

namespace SecurityTestApp
{
    public static class FileManager
    {
        private static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");
        private static Dictionary<string, int> failedAttempts = new Dictionary<string, int>();
        private static Dictionary<string, DateTime> lockoutEnds = new Dictionary<string, DateTime>();

        public static void InitializeFile()
        {
            if (!File.Exists(filePath))
            {
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

            string salt = SecurityHelper.GenerateSalt();
            string hash = SecurityHelper.ComputeHash(password, salt);

            File.AppendAllText(filePath, $"{username}|{salt}|{hash}|{role}\n");
            return true;
        }

        public static string Authenticate(string username, string password)
        {
            if (lockoutEnds.ContainsKey(username))
            {
                if (DateTime.Now < lockoutEnds[username])
                {
                    TimeSpan remaining = lockoutEnds[username] - DateTime.Now;
                    return $"LOCKED|{Math.Ceiling(remaining.TotalMinutes)}";
                }
                else
                {
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
                    string inputHash = SecurityHelper.ComputeHash(password, parts[1]);
                    if (inputHash == parts[2])
                    {
                        failedAttempts[username] = 0;
                        return parts[3];
                    }
                }
            }

            if (!failedAttempts.ContainsKey(username)) failedAttempts[username] = 0;
            failedAttempts[username]++;

            if (failedAttempts[username] >= 3)
            {
                lockoutEnds[username] = DateTime.Now.AddMinutes(5);
                return "LOCKED|5";
            }

            return null;
        }
    }
}