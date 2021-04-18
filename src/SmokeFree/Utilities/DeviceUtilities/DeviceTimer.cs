using SmokeFree.Abstraction.Utility.DeviceUtilities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmokeFree.Utilities.DeviceUtilities
{
    /// <summary>
    /// Device Timer Concrete
    /// </summary>
    public class DeviceTimer : IDeviceTimer
    {
        /// <summary>
        /// Start Timer
        /// </summary>
        /// <param name="func">Function to execute. Return false to stop timer</param>
        /// <param name="cts">Cancelation Token Source. Manages Stop Method</param>
        public void Start(Func<Task<bool>> func, CancellationTokenSource cts)
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (cts.IsCancellationRequested)
                {

                    return false;
                }

                return func.Invoke().Result;

            });
        }

        /// <summary>
        /// Stop Timer
        /// </summary>
        /// <param name="cts">Cancelation Token Source. Stops timer with same instance of cts</param>
        public void Stop(CancellationTokenSource cts)
        {
            Interlocked.Exchange(ref cts, new CancellationTokenSource()).Cancel();
        }
    }
}
