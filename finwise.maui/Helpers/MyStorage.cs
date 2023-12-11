using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.Json;
using finwise.maui.Models;

namespace finwise.maui.Helpers
{ 
    internal class MyStorage
    {
        public MyStorage(){}

        public static string Init<T>(string fileName) where T: BaseModel, new()
        {
            try
            {
                var filepath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                if (!File.Exists(filepath))
                {
                    using (var filestream = File.Create(filepath))
                    {
                        var defaultItems = new List<T>();
                        JsonSerializer.Serialize<List<T>>(filestream, defaultItems);
                        //XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                        //serializer.Serialize(filestream, defaultItems);

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

        public static List<T> LoadFromDataFile<T>() where T : BaseModel, new()
        {
            try
            {
                var itemType = typeof(T);
                List<T> items = new List<T>();
                var filepath = Init<T>($"{itemType.Name}s.json");

                if (File.Exists(filepath))
                {
                    using (FileStream filestream = new FileStream(filepath, FileMode.Open))
                    {
                        //XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                        //items = serializer.Deserialize(filestream) as List<T>;

                        using (StreamReader reader = new StreamReader(filestream))
                        {
                            string jsonContent = reader.ReadToEnd();
                            items = JsonSerializer.Deserialize<List<T>>(jsonContent);
                        }
                    }
                }

                return items;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading file: {ex.Message}");
                return new List<T>();
            }
        }

        public static bool WriteToDataFile<T>(List<T> items) where T : BaseModel, new()
        {
            try
            {
                var itemType = typeof(T);
                Debug.WriteLine($"Item Type file: itemType");

                var filepath = Init<T>($"{itemType.Name}s.json");

                if (filepath is not null)
                {
                    using (FileStream filestream = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        //XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                        //serializer.Serialize(fs, items);

                        JsonSerializer.Serialize<List<T>>(filestream, items);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading file: {ex.Message}");
            }
            return false;
        }
    }
}
