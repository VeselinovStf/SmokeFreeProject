using System;
using System.Globalization;
using Xamarin.Forms;

namespace SmokeFree.Converters
{
    public class TimerToBoleanVisabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeValue = (TimeSpan)value;


            return timeValue == new TimeSpan(0, 0, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
