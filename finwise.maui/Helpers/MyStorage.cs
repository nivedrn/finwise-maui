using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using finwise.maui.Models;

namespace finwise.maui.Helpers
{ 
    internal class MyStorage
    {
        public MyStorage(){}

        public static async Task<string> Init<T>(string fileName) where T: BaseModel, new()
        {
            try
            {
                var mainDir = FileSystem.Current.AppDataDirectory;
                var filepath = Path.Combine(mainDir, fileName);
                if (!File.Exists(filepath))
                {
                    await Logger.WriteLogsAsync("Creating File: " + filepath);
                    using (var filestream = File.Create(filepath))
                    {
                        var defaultItems = new List<T>();
                        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                        serializer.Serialize(filestream, defaultItems);
                    }
                    Debug.WriteLine(fileName + "File Created at Path" + filepath);
                }
                return filepath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating file: {ex.Message}");
            }

            return null;
        }

        public static async Task<List<T>> LoadFromDataFile<T>() where T : BaseModel, new()
        {
            try
            {
                var itemType = typeof(T);
                List<T> items = new List<T>();
                var filepath = await Init<T>($"{itemType.Name}s.xml");

                if (File.Exists(filepath))
                {
                    await Logger.WriteLogsAsync("Reading File: " + filepath);

                    using (FileStream fs2 = new FileStream(filepath, FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                        items = serializer.Deserialize(fs2) as List<T>;
                    }
                }
                await Logger.WriteLogsAsync($"Found { items.Count } for {itemType.Name} ");
                return items;
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine($"File not found: {ex.FileName}");
                return new List<T>();
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading file: {ex.Message}");
                return new List<T>();
            }
        }

        public static async Task<bool> WriteToDataFile<T>(List<T> items) where T : BaseModel, new()
        {
            try
            {
                var itemType = typeof(T);
                Debug.WriteLine($"Item Type file: itemType");

                var filepath = await Init<T>($"{itemType.Name}s.xml");

                if (filepath is not null)
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                        serializer.Serialize(fs, items);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading file: {ex.Message}");
                return false;
            }
        }
    }
}
