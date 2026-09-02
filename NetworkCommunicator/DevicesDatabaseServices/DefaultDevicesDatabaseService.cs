using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.DevicesDatabaseServices.DTOs;
using System.Runtime.Serialization;
using System.Xml;

namespace NetworkCommunicator.DevicesDatabaseServices
{
    internal class DefaultDevicesDatabaseService(string appDirectory, IXmlFileStore fileStore) : IDevicesDatabaseService
    {
        private static readonly string RootElementName = "NetworkDetails";
        private static readonly DataContractSerializerSettings SerializerSettings = new()
        {
            RootName = new XmlDictionaryString(XmlDictionary.Empty, RootElementName, 0)
        };

        private readonly string _savePath = Path.Combine(appDirectory, $"{RootElementName}.xml");
        private readonly IXmlFileStore _fileStore = fileStore;

        public List<NetworkDetail> GetAllDevices()
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(List<NetworkDetailXmlDto>), SerializerSettings);
                using var reader = _fileStore.CreateXmlReader(_savePath);
                var loadedData = (List<NetworkDetailXmlDto>?)serializer.ReadObject(reader)
                    ?? throw new FileLoadException("The XML was not loaded correctly.");
                return [.. loadedData.Select(NetworkDetailXmlDto.FromDto)];
            }
            catch
            {
                // attempt to remove a possibly corrupted file, ignore if it doesn't exist
                try { _fileStore.Delete(_savePath); } catch { }
                throw;
            }
        }

        public void SaveDevices(List<NetworkDetail> devices)
        {
            var serializer = new DataContractSerializer(typeof(List<NetworkDetailXmlDto>), SerializerSettings);
            using var writer = _fileStore.CreateXmlWriter(_savePath, new XmlWriterSettings { Indent = true });
            serializer.WriteObject(writer, devices.Select(NetworkDetailXmlDto.ToDto).ToList());
        }
    }
}
