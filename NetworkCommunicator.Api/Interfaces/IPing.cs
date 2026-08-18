using System.Net;
using System.Net.NetworkInformation;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface IPing : IDisposable
    {
        Task<PingReply> SendPingAsync(IPAddress address);
    }
}
