using NetworkCommunicator.Api.Enums;
using System.Net;
using System.Net.NetworkInformation;
using System.Xml.Serialization;

namespace NetworkCommunicator.Api.Entities
{
    [Serializable]
    public class NetworkDetail
    {
        [XmlElement]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public IPAddress IpAddress { get; set; } = IPAddress.None;

        [XmlElement("IpAddress")]
        public string IpAddressXml
        {
            get => IpAddress.ToString();
            set => IpAddress = string.IsNullOrWhiteSpace(value) ? IPAddress.None : IPAddress.Parse(value);
        }

        [XmlIgnore]
        public PhysicalAddress MacAddress { get; set; } = PhysicalAddress.None;


        [XmlElement("MacAddress")]
        public string MacAddressXml
        {
            get => MacAddress.ToString();
            set => MacAddress = string.IsNullOrWhiteSpace(value) ? PhysicalAddress.None : PhysicalAddress.Parse(value);
        }

        [XmlIgnore]
        public DeviceStatus Status { get; set; } = DeviceStatus.Offline;
    }
}
