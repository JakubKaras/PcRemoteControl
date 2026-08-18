using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Enums;
using NetworkCommunicator.Api.Interfaces;
using System.Net.NetworkInformation;

namespace NetworkCommunicator.PingHandlers
{
    internal class DefaultPingHandler(IPingFactory pingFactory) : IPingHandler
    {
        public async Task<DeviceStatus> Ping(NetworkDetail networkDetail, IProgress<DeviceStatus> progress)
        {
            var isOnline = false;

            try
            {
                using IPing pinger = pingFactory.Create();
                progress.Report(DeviceStatus.Loading);
                var reply = await pinger.SendPingAsync(networkDetail.IpAddress);
                isOnline = reply != null && reply.Status == IPStatus.Success;
            }
            catch
            {
                isOnline = false;
            }
            
            var finalStatus = isOnline ? DeviceStatus.Online : DeviceStatus.Offline;

            progress.Report(finalStatus);
            return finalStatus;
        }
    }
}
