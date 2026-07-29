using PcRemoteControl.Entities;

namespace PcRemoteControl.Models
{
    public class AddOrEditDeviceViewModel(NetworkDetail device, bool isEdit)
    {
        public NetworkDetail Device { get; init; } = device;

        public string Title { get => isEdit ? "Editing device" : "Adding new device"; }
    }
}
