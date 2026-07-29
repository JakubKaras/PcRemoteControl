using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.DevicesDatabaseServices.DTOs;
using System.Runtime.Serialization;
using System.Xml;

namespace NetworkCommunicator.DevicesDatabaseServices
{
    internal class DefaultDevicesDatabaseService(string appDirectory) : IDevicesDatabaseService
    {
        private static readonly string RootElementName = "NetworkDetails";
        private static readonly DataContractSerializerSettings SerializerSettings = new()
        {
            RootName = new XmlDictionaryString(XmlDictionary.Empty, RootElementName, 0)
        };

        private readonly string _savePath = Path.Combine(appDirectory, $"{RootElementName}.xml");

        public List<NetworkDetail> GetAllDevices()
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(List<NetworkDetailXmlDto>), SerializerSettings);
                using var reader = XmlReader.Create(_savePath);
                var loadedData = (List<NetworkDetailXmlDto>?)serializer.ReadObject(reader)
                    ?? throw new FileLoadException("The XML was not loaded correctly.");
                return [.. loadedData.Select(NetworkDetailXmlDto.FromDto)];
            }
            catch (Exception e)
            {
                File.Delete(_savePath);
                throw;
            }
        }

        public void SaveDevices(List<NetworkDetail> devices)
        {
            var serializer = new DataContractSerializer(typeof(List<NetworkDetailXmlDto>), SerializerSettings);
            using var writer = XmlWriter.Create(_savePath, new XmlWriterSettings { Indent = true });
            serializer.WriteObject(writer, devices.Select(NetworkDetailXmlDto.ToDto).ToList());
        }
    }
}
