using System.Globalization;
using System.Windows.Data;

namespace BigFileReader.Converters
{
    internal class ConverterDoubleToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = (int)value;

            return System.Convert.ToDouble(intValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;

            return System.Convert.ToDouble(doubleValue);
        }
    }
}
