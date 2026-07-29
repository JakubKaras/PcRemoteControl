using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Enums;
using NetworkCommunicator.Api.Interfaces;
using System.Net.NetworkInformation;

namespace NetworkCommunicator.PingHandlers
{
    internal class DefaultPingHandler : IPingHandler
    {
        public async Task<DeviceStatus> Ping(NetworkDetail networkDetail, IProgress<DeviceStatus> progress)
        {
            var isOnline = false;
            var currentStatus = DeviceStatus.Loading;

            try
            {
                using Ping pinger = new();
                progress.Report(currentStatus);
                var reply = await pinger.SendPingAsync(networkDetail.IpAddress);
                isOnline = reply.Status == IPStatus.Success;
            }
            finally
            {
                currentStatus = isOnline ? DeviceStatus.Online : DeviceStatus.Offline;
            }

            progress.Report(currentStatus);
            return currentStatus;
        }
    }
}
