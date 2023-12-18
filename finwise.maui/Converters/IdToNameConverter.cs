using finwise.maui.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Converters
{
    public class IdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string id)
            {
                if( id != App._settings["userId"])
                {
                    // Look for the Person with the specified ID
                    Person person = App._people.Find(p => p.id == id);

                    // If the person is found, return their name, otherwise return the ID
                    return person?.name ?? id;
                }
                else
                {
                    return $"{App._settings["username"]} (You)" ?? id;
                }
            }

            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}