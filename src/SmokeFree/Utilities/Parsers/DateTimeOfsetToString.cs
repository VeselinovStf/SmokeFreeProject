using System;

namespace SmokeFree.Utilities.Parsers
{
    public static class DateTimeOfsetToString
    {
        public static string DateTime(DateTimeOffset offset)
        {
            return offset.LocalDateTime.ToString("dd/MM/yyyy");
        }
    }
}
