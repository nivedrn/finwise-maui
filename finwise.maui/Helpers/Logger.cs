using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Helpers
{
    public static class Logger
    {
        public static string filePath = Path.Combine(FileSystem.AppDataDirectory, "logs.txt");

        public static async Task WriteLogsAsync(string message)
        {
            await File.AppendAllTextAsync(filePath, $"{DateTime.Now.ToString("dd-MM-yyyy")}:{message}\n");
        }

        public static async Task<string> ReadLogsAsync()
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
