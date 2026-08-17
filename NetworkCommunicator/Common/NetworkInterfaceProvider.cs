using NetworkCommunicator.Api.Interfaces;
using System.Net.NetworkInformation;

namespace NetworkCommunicator.Common
{
    internal class NetworkInterfaceProvider : INetworkInterfaceProvider
    {
        public IEnumerable<NetworkInterface> GetAllNetworkInterfaces() => NetworkInterface.GetAllNetworkInterfaces();
    }
}
