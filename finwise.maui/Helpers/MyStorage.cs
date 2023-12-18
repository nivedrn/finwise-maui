using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.Json;
using finwise.maui.Models;
using Org.Json;

namespace finwise.maui.Helpers
{ 
    internal class MyStorage
    {
        public MyStorage(){}

        public static bool toDelete = false;
        public static string Init<T>(string fileName) where T: BaseModel, new()
        {
            try
            {
                var filepath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                if (toDelete)
                {
                    File.Delete(filepath);
                    //toDelete = false;
                }

                if (!File.Exists(filepath))
                {
                    using (var filestream = File.Create(filepath))
                    {
                        var defaultItems = new List<T>();
                        JsonSerializer.Serialize<List<T>>(filestream, defaultItems);
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
                        string jsonString = JsonSerializer.Serialize<List<T>>(items);
                        JsonSerializer.Serialize(filestream, items);
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

        public static Dictionary<string, string> LoadAppSettings()
        {
            var jsonString = "";
            if (Preferences.Default.ContainsKey("Settings"))
            {
                //Preferences.Default.Set("Settings", string.Empty); //Uncomment to reset Settings
                jsonString =  Preferences.Default.Get("Settings", string.Empty);
            }

            Dictionary<string, string> defaultSettings = new Dictionary<string, string>(); 

            if (jsonString != "")
            {
                defaultSettings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
            }

            if(defaultSettings.Count == 0)
            {
                defaultSettings["userId"] = Guid.NewGuid().ToString();
                defaultSettings["username"] = "User";
                defaultSettings["monthlyBudget"] = "1000";
                defaultSettings["budgetStartDay"] = "12";
                defaultSettings["currentCountryName"] = "Germany";
                defaultSettings["currentCurrencyCode"] = "EUR";
                defaultSettings["currentCurrencySymbol"] = "€";
            }

            return defaultSettings;
        }

        public static void SaveAppSettings(Dictionary<string, string> settings)
        {
            string json = JsonSerializer.Serialize(settings);

            Preferences.Default.Set("Settings", json);
        }
    }
}
