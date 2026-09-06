using System.Net;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface IUdpClientFactory
    {
        IUdpClient Create(IPEndPoint localEndPoint);
    }
}
