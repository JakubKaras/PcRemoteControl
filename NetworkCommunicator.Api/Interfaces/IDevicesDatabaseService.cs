using NetworkCommunicator.Api.Entities;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface IDevicesDatabaseService
    {
        List<NetworkDetail> GetAllDevices();

        void SaveDevices(List<NetworkDetail> devices);
    }
}
