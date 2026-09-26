using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Interfaces;
using System.Net.NetworkInformation;
using System.Net;

namespace NetworkCommunicator.WakeUpHandlers
{
    internal class DefaultWakeUpHandler(IUdpClientFactory udpClientFactory) : IWakeUpHandler
    {
        private static readonly IPEndPoint _broadcastEndpoint = new(IPAddress.Broadcast, 7);

        public async Task WakeUp(NetworkDetail device)
        {
            byte[] magicPacket = BuildMagicPacket(device.MacAddress);
            await SendMagicPacket(magicPacket);
        }

        private static byte[] BuildMagicPacket(PhysicalAddress macAddress)
        {
            IEnumerable<byte> header = Enumerable.Repeat((byte)0xff, 6);
            IEnumerable<byte> data = Enumerable.Repeat(macAddress.GetAddressBytes(), 16).SelectMany(x => x);

            return [.. header, .. data];
        }

        private async Task SendMagicPacket(byte[] magicPacket)
        {
            using var udpClient = udpClientFactory.Create();
            await udpClient.SendAsync(magicPacket, magicPacket.Length, _broadcastEndpoint);
        }
    }
}
