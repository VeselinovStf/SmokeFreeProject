using SmokeFree.Abstraction.Utility.Wrappers;
using System;

namespace SmokeFree.Utilities.Wrappers
{
    /// <summary>
    /// App DateTime Wrapper
    /// </summary>
    public class DateTimeWrapper : IDateTimeWrapper
    {
        /// <summary>
        /// Get DateTime Now for App
        /// </summary>
        /// <returns>App DateTime Representation - DateTime.Now</returns>
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
