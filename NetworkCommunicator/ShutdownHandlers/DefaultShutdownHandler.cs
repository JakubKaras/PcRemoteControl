using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Interfaces;
using System.Net.Sockets;

namespace NetworkCommunicator.ShutdownHandlers
{
    internal class DefaultShutdownHandler(ISocketFactory socketFactory) : IShutdownHandler
    {
        const int ShutdownPort = 9110;
        private static readonly byte[] _shutdowMessage = [115, 104, 117, 116, 100, 111, 119, 110, 10];

        public int Shutdown(NetworkDetail network)
        {
            try
            {
                using ISocket socket = socketFactory.Create(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(network.IpAddress, ShutdownPort);
                int result = socket.Send(_shutdowMessage);
                return result;
            }
            catch
            {
                return -1;
            }
        }
    }
}
