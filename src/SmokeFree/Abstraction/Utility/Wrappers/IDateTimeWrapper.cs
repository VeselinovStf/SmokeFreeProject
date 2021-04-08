using System;

namespace SmokeFree.Abstraction.Utility.Wrappers
{
    /// <summary>
    /// DateTime Wrapper Abstraction
    /// </summary>
    public interface IDateTimeWrapper
    {
        /// <summary>
        /// Get DateTime Now for App
        /// </summary>
        /// <returns>App DateTime Representation</returns>
        DateTime Now();
    }
}
