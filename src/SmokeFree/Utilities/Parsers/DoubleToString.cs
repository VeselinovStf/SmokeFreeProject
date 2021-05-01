using System;

namespace SmokeFree.Utilities.Parsers
{
    public static class DoubleToString
    {
        public static string DateTime(double value)
        {
            var time = new TimeSpan(0, 0, (int)value);

            var formatTime = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);

            return string.Format("{0:hh\\:mm\\:ss}", formatTime);
        }

        public static string Procent(double value)
        {
            return string.Format("{0:N2}", value);
        }
    }
}
