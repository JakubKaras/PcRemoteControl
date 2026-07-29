using CommunityToolkit.Mvvm.ComponentModel;
using Google.Android.Material.Color.Utilities;
using NetworkCommunicator.Api.Enums;
using System.Net;
using System.Net.NetworkInformation;
using ApiNetworkDetail = NetworkCommunicator.Api.Entities.NetworkDetail;

namespace PcRemoteControl.Entities
{
    public partial class NetworkDetail : ObservableObject
    {
        [ObservableProperty]
        public partial string Name { get; set; }

        [ObservableProperty]
        public partial string IpAddress { get; set; }

        [ObservableProperty]
        public partial string MacAddress { get; set; }

        [ObservableProperty]
        public partial DeviceStatus Status { get; set; }

        private NetworkDetail(string name, string ipAddress, string macAddress, DeviceStatus status)
        {
            Name = name;
            IpAddress = ipAddress;
            MacAddress = macAddress;
            Status = status;
        }

        public NetworkDetail(string name, string ipAddress, string macAddress) : this(name, ipAddress, macAddress, DeviceStatus.Offline)
        {
        }

        public NetworkDetail()
        {
            Name = string.Empty;
            IpAddress = string.Empty;
            MacAddress = string.Empty;
            Status = DeviceStatus.Offline;
        }

        internal static NetworkDetail FromApi(ApiNetworkDetail apiDetail)
        {
            return new NetworkDetail(apiDetail.Name, apiDetail.IpAddress.ToString(), string.Join(":", apiDetail.MacAddress.GetAddressBytes().Select(b => b.ToString("X2"))), apiDetail.Status);
        }

        internal static ApiNetworkDetail ToApi(NetworkDetail detail)
        {
            return new ApiNetworkDetail { 
                Name = detail.Name, 
                IpAddress = IPAddress.Parse(detail.IpAddress),
                MacAddress = PhysicalAddress.Parse(detail.MacAddress),
                Status = detail.Status
            };
        }
    }
}
