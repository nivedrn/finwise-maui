using finwise.maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        //public static async Task<List<Expense>> readExpenseFile()
        //{
        //    List<Expense> items = new List<Expense>();
        //    var fp = Path.Combine(FileSystem.AppDataDirectory, "Expenses.xml");
        //    using (FileStream fs2 = new FileStream(fp, FileMode.Open))
        //    {
        //        XmlSerializer serializer = new XmlSerializer(typeof(List<Expense>));
        //        items = serializer.Deserialize(fs2) as List<Expense>;
        //    }

        //    return items;
        //}
    }
}
