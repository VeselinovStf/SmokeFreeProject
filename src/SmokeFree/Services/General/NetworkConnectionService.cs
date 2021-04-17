using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using SmokeFree.Abstraction.Services.General;

namespace SmokeFree.Services.General
{
    /// <summary>
    /// Network Connection Service
    /// </summary>
    public class NetworkConnectionService : INetworkConnectionService
    {
        private readonly IConnectivity _connectivity;

        public NetworkConnectionService()
        {
            _connectivity = CrossConnectivity.Current;
            _connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            ConnectivityChanged?.Invoke(this, new ConnectivityChangedEventArgs() { IsConnected = e.IsConnected });
        }

        /// <summary>
        /// Is device Connected to internet Network
        /// </summary>
        public bool IsConnected => _connectivity.IsConnected;

        /// <summary>
        /// Network Connection Changed Event
        /// </summary>
        public event ConnectivityChangedEventHandler ConnectivityChanged;
    }
}
