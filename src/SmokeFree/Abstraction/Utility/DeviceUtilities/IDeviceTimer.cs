using System;
using System.Threading;

namespace SmokeFree.Abstraction.Utility.DeviceUtilities
{
    /// <summary>
    /// Device Timer Abstraction
    /// </summary>
    public interface IDeviceTimer
    {
        /// <summary>
        /// Start Timer
        /// </summary>
        /// <param name="func">Function to execute. Return false to stop timer</param>
        /// <param name="cts">Cancelation Token Source. Manages Stop Method</param>
        void Start(Func<bool> p, CancellationTokenSource stopTestingTimerCancellation);

        /// <summary>
        /// Stop Timer
        /// </summary>
        /// <param name="cts">Cancelation Token Source. Stops timer with same instance of cts</param>
        void Stop(CancellationTokenSource stopTestingTimerCancellation);
    }
}
