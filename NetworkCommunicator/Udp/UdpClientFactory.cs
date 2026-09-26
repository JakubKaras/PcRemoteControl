using NetworkCommunicator.Api.Interfaces;
using System.Net;

namespace NetworkCommunicator.Udp
{
    internal sealed class UdpClientFactory : IUdpClientFactory
    {
        public IUdpClient Create() => new UdpClientAdapter();
    }
}
