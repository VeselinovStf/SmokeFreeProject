using SmokeFree.Abstraction.Utility.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SmokeFree.Utilities.Logging
{
    /// <summary>
    /// Debugging Logger
    /// </summary>
    public class DebugLogger : IAppLogger
    {
        public void LogCritical(string message, [CallerMemberName] string caller = null)
        {
            Debug.WriteLine($"[CRITICAL] {caller} : {message}");
        }

        public void LogError(string message, [CallerMemberName] string caller = null)
        {
            Debug.WriteLine($"[ERROR] {caller} : {message}");

        }

        public void LogInformation(string message, [CallerMemberName] string caller = null)
        {
            Debug.WriteLine($"[INFO] {caller} : {message}");

        }

        public void LogWarning(string message, [CallerMemberName] string caller = null)
        {
            Debug.WriteLine($"[WARNING] {caller} : {message}");

        }
    }
}
