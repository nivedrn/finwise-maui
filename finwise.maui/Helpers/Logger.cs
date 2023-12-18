using finwise.maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace finwise.maui.Helpers
{
    public static class Logger
    {
        public static string filePath = Path.Combine(FileSystem.AppDataDirectory, "Expenses.json");

        public static async Task WriteLogsAsync(string message)
        {
            await File.AppendAllTextAsync(filePath, $"{DateTime.Now.ToString("dd-MM-yyyy")}:{message}\n");
        }

        public static async Task<string> ReadLogsAsync() { 
        
            var jsonstring = await File.ReadAllTextAsync(filePath);

            List<Expense> items = JsonSerializer.Deserialize<List<Expense>>(jsonstring);
            return jsonstring;
        }

    }
}
