using NetworkCommunicator.Api.Interfaces;
using System.Net;

namespace NetworkCommunicator.Udp
{
    internal sealed class UdpClientFactory : IUdpClientFactory
    {
        public IUdpClient Create(IPEndPoint localEndPoint) => new UdpClientAdapter(localEndPoint);
    }
}
