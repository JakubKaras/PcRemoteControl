using NetworkCommunicator.Api.Interfaces;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkCommunicator.PingHandlers
{
    internal class PingAdapter : IPing
    {
        private readonly Ping _pinger = new();

        public Task<PingReply> SendPingAsync(IPAddress address)
        {
            return _pinger.SendPingAsync(address);
        }

        public void Dispose()
        {
            _pinger.Dispose();
        }
    }
}
