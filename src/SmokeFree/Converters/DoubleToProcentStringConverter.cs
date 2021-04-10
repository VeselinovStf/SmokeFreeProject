using System;
using System.Globalization;
using Xamarin.Forms;

namespace SmokeFree.Converters
{
    /// <summary>
    /// Convert boolean value to string with % sign
    /// </summary>
    public class DoubleToProcentStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;

            return string.Format("{0:N2}", doubleValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
