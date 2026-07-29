using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Interfaces;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace NetworkCommunicator.WakeUpHandlers
{
    internal class DefaultWakeUpHandler : IWakeUpHandler
    {
        private static readonly IPEndPoint _multicastEndpoint = new(new IPAddress([224, 0, 0, 1]), 7);

        public async Task WakeUp(NetworkDetail device)
        {
            byte[] magicPacket = BuildMagicPacket(device.MacAddress);

            IEnumerable<NetworkInterface> interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            foreach (NetworkInterface nic in interfaces)
            {
                IPInterfaceProperties interfaceProperties = nic.GetIPProperties();
                UnicastIPAddressInformation? unicastIPAddressInformation = interfaceProperties.UnicastAddresses
                    .FirstOrDefault(u => u.Address.AddressFamily == AddressFamily.InterNetwork);
                if (unicastIPAddressInformation != null)
                {
                    await SendMagicPacket(magicPacket, unicastIPAddressInformation.Address);
                    return;
                }
            }
        }

        private static byte[] BuildMagicPacket(PhysicalAddress macAddress)
        {
            IEnumerable<byte> header = Enumerable.Repeat((byte)0xff, 6);
            IEnumerable<byte> data = Enumerable.Repeat(macAddress.GetAddressBytes(), 16).SelectMany(x => x);

            return [.. header, .. data];
        }

        private static async Task SendMagicPacket(byte[] magicPacket, IPAddress localIpAddress)
        {
            using UdpClient udpClient = new(new IPEndPoint(localIpAddress, 0));
            await udpClient.SendAsync(magicPacket, magicPacket.Length, _multicastEndpoint);
        }
    }
}
