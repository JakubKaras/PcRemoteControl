using System.Net;
using System.Net.Sockets;
using NetworkCommunicator.Api.Interfaces;

namespace NetworkCommunicator.Udp
{
    internal sealed class UdpClientAdapter(IPEndPoint localEndPoint) : IUdpClient
    {
        private readonly UdpClient _udpClient = localEndPoint is null ? new UdpClient() : new UdpClient(localEndPoint);

        public async Task SendAsync(byte[] buffer, int bytes, IPEndPoint endpoint)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            ArgumentNullException.ThrowIfNull(endpoint);

            await _udpClient.SendAsync(buffer, bytes, endpoint).ConfigureAwait(false);
        }

        public void Dispose() => _udpClient.Dispose();
    }
}
