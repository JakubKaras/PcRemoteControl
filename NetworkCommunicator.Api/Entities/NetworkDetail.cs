using NetworkCommunicator.Api.Enums;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkCommunicator.Api.Entities
{
    public class NetworkDetail
    {
        public string Name { get; set; } = string.Empty;

        public IPAddress IpAddress { get; set; } = IPAddress.None;

        public PhysicalAddress MacAddress { get; set; } = PhysicalAddress.None;

        public DeviceStatus Status { get; set; } = DeviceStatus.Offline;
    }
}
