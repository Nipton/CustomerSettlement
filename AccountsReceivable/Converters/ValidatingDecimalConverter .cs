using System;
using System.Globalization;
using System.Windows.Data;

namespace AccountsReceivable.Converters
{
    public class ValidatingDecimalConverter : IValueConverter
    {
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                if (input.EndsWith(".") || input.EndsWith(","))
                {
                    return Binding.DoNothing;
                }
                if (culture.NumberFormat.NumberDecimalSeparator == ",")
                    input = input.Replace('.', ',');
                if (decimal.TryParse(input, NumberStyles.Any, culture, out var result))
                    return result;
            }
            return null;
        }
              
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d)
            {
                return d.ToString(culture);
            }
            if (value == null)
                return Binding.DoNothing;
            return string.Empty;    
        }
    }
}
