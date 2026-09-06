using System.Net;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface IUdpClient : IDisposable
    {
        Task SendAsync(byte[] buffer, int bytes, IPEndPoint endpoint);
    }
}
