using System.Runtime.CompilerServices;

namespace SmokeFree.Abstraction.Utility.Logging
{
    /// <summary>
    /// Logger Abstraction
    /// </summary>
    public interface IAppLogger
    {
        void LogCritical(string message, [CallerMemberName] string caller = null);


        void LogError(string message, [CallerMemberName] string caller = null);


        void LogInformation(string message, [CallerMemberName] string caller = null);


        void LogWarning(string message, [CallerMemberName] string caller = null);

    }
}
