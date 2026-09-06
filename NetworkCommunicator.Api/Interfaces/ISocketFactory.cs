using System.Net.Sockets;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface ISocketFactory
    {
        ISocket Create(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType);
    }
}
