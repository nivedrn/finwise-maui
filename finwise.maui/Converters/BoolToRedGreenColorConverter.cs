using Microsoft.Maui.Graphics.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Converters
{
    public class BoolToRedGreenColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColorTypeConverter converter = new ColorTypeConverter();

            if (value is bool boolValue)
            {
                return boolValue ? Color.FromRgb(25, 111, 61) : Color.FromRgb(144, 12, 63);//(Color)(converter.ConvertFromInvariantString("red")); ;
            }

            return Color.FromRgb(25, 111, 61);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
