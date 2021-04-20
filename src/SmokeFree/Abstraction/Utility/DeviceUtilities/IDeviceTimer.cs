using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmokeFree.Abstraction.Utility.DeviceUtilities
{
    /// <summary>
    /// Device Timer Abstraction
    /// </summary>
    public interface IDeviceTimer
    {
        /// <summary>
        /// Async Start Timer
        /// </summary>
        /// <param name="func">Function to execute. Return false to stop timer</param>
        /// <param name="cts">Cancelation Token Source. Manages Stop Method</param>
        void Start(Func<Task<bool>> p, CancellationTokenSource cts);

        /// <summary>
        /// Start Timer
        /// </summary>
        /// <param name="func">Function to execute. Return false to stop timer</param>
        /// <param name="cts">Cancelation Token Source. Manages Stop Method</param>
        void Start(Func<bool> p, CancellationTokenSource cts);

        /// <summary>
        /// Stop Timer
        /// </summary>
        /// <param name="cts">Cancelation Token Source. Stops timer with same instance of cts</param>
        void Stop(CancellationTokenSource cts);
    }
}
