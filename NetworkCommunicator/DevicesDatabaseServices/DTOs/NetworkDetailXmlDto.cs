using NetworkCommunicator.Api.Entities;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;

namespace NetworkCommunicator.DevicesDatabaseServices.DTOs
{
    [DataContract(Name = "NetworkDetail", Namespace = "")]
    internal class NetworkDetailXmlDto
    {
        [DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public string IpAddress { get; set; } = string.Empty;

        [DataMember]
        public string MacAddress { get; set; } = string.Empty;

        internal static NetworkDetailXmlDto ToDto(NetworkDetail core) => new()
        {
            Name = core.Name,
            IpAddress = core.IpAddress.ToString(),
            MacAddress = core.MacAddress.ToString(),
        };

        internal static NetworkDetail FromDto(NetworkDetailXmlDto dto) => new()
        {
            Name = dto.Name,
            IpAddress = string.IsNullOrWhiteSpace(dto.IpAddress) ? IPAddress.None : IPAddress.Parse(dto.IpAddress),
            MacAddress = string.IsNullOrWhiteSpace(dto.MacAddress) ? PhysicalAddress.None : PhysicalAddress.Parse(dto.MacAddress),
        };
    }
}
