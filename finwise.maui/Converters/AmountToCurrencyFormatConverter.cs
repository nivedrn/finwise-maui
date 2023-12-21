using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Converters
{
    class AmountToCurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                return $"{App._settings["currentCurrencySymbol"]} {amount.ToString("0.00")}";
            }

            if (value is string amountString)
            {
                return $"{App._settings["currentCurrencySymbol"]} {amountString}";
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
