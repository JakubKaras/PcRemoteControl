using NetworkCommunicator.Api.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace NetworkCommunicator.Sockets
{
    internal class SocketAdapter(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : ISocket
    {
        private readonly Socket _socket = new(addressFamily, socketType, protocolType);

        public void Connect(IPAddress address, int port)
        {
            _socket.Connect(address, port);
        }

        public void Dispose()
        {
            _socket.Dispose();
        }

        public int Send(byte[] buffer) => _socket.Send(buffer);
    }
}
