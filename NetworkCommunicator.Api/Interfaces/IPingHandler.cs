using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Enums;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface IPingHandler
    {
        Task<DeviceStatus> Ping(NetworkDetail networkDetail, IProgress<DeviceStatus> progress);
    }
}
