using System;
using System.Globalization;
using Xamarin.Forms;

namespace SmokeFree.Converters
{
    /// <summary>
    /// Convert TimeSpan? to static Time Display String
    /// </summary>
    public class TimeSpanToStaticTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timestamp = value as TimeSpan?;


            if (timestamp == null)
            {
                return "0";
            }

            var time = timestamp.GetValueOrDefault();

            var formatTime = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);

            return string.Format("{0:hh\\:mm\\:ss}", formatTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
