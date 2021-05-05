using System;
using System.IO;
using System.Reflection;


namespace SimpleAPI
{
    class Env
    {
        public static string User1 => Environment.GetEnvironmentVariable("USER1");
        public static string Password1 => Environment.GetEnvironmentVariable("PASSWORD1");
        public static string User2 => Environment.GetEnvironmentVariable("USER2");
        public static string Password2 => Environment.GetEnvironmentVariable("PASSWORD2");
        public static void Initialize()
        {
            var projectPath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            var env = Path.GetFullPath(Path.Combine(projectPath, @"..\..\..\..\.env"));
            foreach (var line in File.ReadAllLines(env))
            {
                var parts = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;
                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}
