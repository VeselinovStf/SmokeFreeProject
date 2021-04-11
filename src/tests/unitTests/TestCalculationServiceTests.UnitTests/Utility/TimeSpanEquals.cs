using System;
using System.Collections.Generic;
using System.Text;

namespace TestCalculationServiceTests.UnitTests.Utility
{
    public static class TimeSpanEquals
    {
        public static bool FormatedEqual(this TimeSpan a, TimeSpan b)
        {
            return (int)a.TotalSeconds == (int)b.TotalSeconds && 
                (int)a.TotalMinutes == (int)b.TotalMinutes && 
                (int)a.TotalHours == (int)b.TotalHours && 
                (int)a.TotalDays == (int)b.TotalDays;
        }
    }
}
