using Plugin.Connectivity.Abstractions;

namespace SmokeFree.Abstraction.Services.General
{
    /// <summary>
    /// Network Connection Service Abstraction
    /// </summary>
    public interface INetworkConnectionService
    {
        /// <summary>
        /// Is device Connected to internet Network
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Network Connection Changed Event
        /// </summary>
        event ConnectivityChangedEventHandler ConnectivityChanged;
    }
}
