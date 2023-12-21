using Microsoft.Maui.Graphics.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value is string boolString)
            {
                switch (boolString)
                {
                    case "true": return true;
                    case "TRUE": return true;
                    case "True": return true;

                    case "false": return false;
                    case "FALSE": return false;
                    case "False": return false;
                }
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "true" : "false";
            }

            return "true";
        }

    }
}
