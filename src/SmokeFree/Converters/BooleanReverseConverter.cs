using System;
using System.Globalization;
using Xamarin.Forms;

namespace SmokeFree.Converters
{
    /// <summary>
    /// Reverse Boolen value true -> false / false -> true
    /// </summary>
    public class BooleanReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;

            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
