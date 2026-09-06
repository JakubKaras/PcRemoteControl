using NetworkCommunicator.Api.Interfaces;

namespace NetworkCommunicator.PingHandlers
{
    internal class PingFactory : IPingFactory
    {
        public IPing Create() => new PingAdapter();
    }
}
