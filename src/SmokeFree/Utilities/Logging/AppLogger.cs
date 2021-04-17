using SmokeFree.Abstraction.Utility.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SmokeFree.Utilities.Logging
{
    /// <summary>
    /// Application Logger
    /// </summary>
    public class AppLogger : IAppLogger
    {
        private string criticalLog = "[CRITICAL] {0} : {1}";
        private string errorlLog = "[ERROR] {0} : {1}";
        private string infolLog = "[INFO] {0} : {1}";
        private string warrningLog = "[WARNING] {0} : {1}";

        private readonly NLog.ILogger LocalLogger = NLog.LogManager.GetCurrentClassLogger();

        public void LogCritical(string message, [CallerMemberName] string caller = null)
        {
            var logMessage = string.Format(criticalLog, caller, message);

            Debug.WriteLine(logMessage);

            LocalLogger.Fatal(logMessage);
        }

        public void LogError(string message, [CallerMemberName] string caller = null)
        {
            var logMessage = string.Format(errorlLog, caller, message);

            Debug.WriteLine(logMessage);

            LocalLogger.Error(logMessage);
        }

        public void LogInformation(string message, [CallerMemberName] string caller = null)
        {
            var logMessage = string.Format(infolLog, caller, message);

            Debug.WriteLine(logMessage);

            LocalLogger.Info(logMessage);
        }

        public void LogWarning(string message, [CallerMemberName] string caller = null)
        {
            var logMessage = string.Format(warrningLog, caller, message);

            Debug.WriteLine(logMessage);

            LocalLogger.Warn(logMessage);
        }
    }
}
