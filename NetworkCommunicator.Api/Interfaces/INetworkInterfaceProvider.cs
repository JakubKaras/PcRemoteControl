using System.Net.NetworkInformation;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface INetworkInterfaceProvider
    {
        IEnumerable<NetworkInterface> GetAllNetworkInterfaces();
    }
}
