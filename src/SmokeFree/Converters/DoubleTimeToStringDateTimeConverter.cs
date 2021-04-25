using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SmokeFree.Converters
{
    public class DoubleTimeToStringDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeValue = (double)value;
            var time = new TimeSpan(0,0,(int)timeValue);

            var formatTime = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);

            return string.Format("{0:hh\\:mm\\:ss}", formatTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
