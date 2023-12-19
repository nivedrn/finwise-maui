using Microsoft.Maui.Graphics.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColorTypeConverter converter = new ColorTypeConverter();
            
            if (value is string statusCode)
            {
                switch (statusCode)
                {
                    case "NOTIFY": return (Color)(converter.ConvertFromInvariantString("green"));
                    case "WARNING": return (Color)(converter.ConvertFromInvariantString("orange"));
                    case "DANGER": return Color.FromRgba(127, 17, 224, 1);
                    case "DEFAULT": return Color.FromRgba(127, 17, 224, 1); 
                    case "OWESYOU": return (Color)(converter.ConvertFromInvariantString("green"));
                    case "OWEDTOYOU": return (Color)(converter.ConvertFromInvariantString("red"));
                }
            }

            return Color.FromRgba(127, 17, 224, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
