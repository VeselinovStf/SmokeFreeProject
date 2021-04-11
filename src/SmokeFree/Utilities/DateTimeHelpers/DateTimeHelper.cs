using System;
using System.Globalization;

namespace SmokeFree.Utilities.DateTimeHelpers
{
    /// <summary>
    /// Date Time Helpers
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Extension Method - Parse DateTimeOffset to DateTime
        /// </summary>
        /// <param name="offset">DateTimeOffset To Parse</param>
        /// <returns>Parsed DateTime or exception</returns>
        /// <exception cref="FormatException">If Cant be Parsed</exception>
        /// <exception cref="ArgumentNullException">If offset is null</exception>
        public static DateTime DateTimeParse(this DateTimeOffset offset)
        {
            return DateTime.Parse(offset.ToString(), CultureInfo.InvariantCulture);
        }
    }
}
