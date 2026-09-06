using System.Net;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface ISocket : IDisposable
    {
        void Connect(IPAddress address, int port);

        int Send(byte[] buffer);
    }
}
