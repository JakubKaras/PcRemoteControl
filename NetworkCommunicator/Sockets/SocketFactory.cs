using NetworkCommunicator.Api.Interfaces;
using System.Net.Sockets;

namespace NetworkCommunicator.Sockets
{
    internal class SocketFactory : ISocketFactory
    {
        public ISocket Create(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) =>
            new SocketAdapter(addressFamily, socketType, protocolType);
    }
}
