using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Moq;
using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.DevicesDatabaseServices;
using NetworkCommunicator.DevicesDatabaseServices.DTOs;

namespace NetworkCommunicator.Tests.DevicesDatabaseServices
{
    public abstract class DefaultDevicesDatabaseServiceTests
    {
        protected readonly Mock<IXmlFileStore> FileStoreMock = new();
        protected readonly static string AppDirectory = Path.GetTempPath();
        protected readonly string ExpectedSavePath = Path.Combine(AppDirectory, "NetworkDetails.xml");
        protected readonly static List<NetworkDetail> SampleDevices =
        [
            new NetworkDetail { Name = "Device1", IpAddress = IPAddress.Loopback, MacAddress = PhysicalAddress.Parse("00:11:22:33:44:55") },
        ];

        protected IDevicesDatabaseService UnderTest;

        protected DefaultDevicesDatabaseServiceTests()
        {
            UnderTest = new DefaultDevicesDatabaseService(AppDirectory, FileStoreMock.Object);
        }

        protected static string SerializeSampleXml(IEnumerable<NetworkDetail> items)
        {
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb);
            using var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true });

            var serializerSettings = new DataContractSerializerSettings
            {
                RootName = new XmlDictionaryString(XmlDictionary.Empty, "NetworkDetails", 0)
            };
            var serializer = new DataContractSerializer(typeof(List<NetworkDetailXmlDto>), serializerSettings);
            serializer.WriteObject(xw, items.Select(NetworkDetailXmlDto.ToDto).ToList());

            xw.Flush();
            return sb.ToString();
        }
    }

    public sealed class When_there_is_an_existing_file_and_it_is_valid : DefaultDevicesDatabaseServiceTests
    {
        [Fact]
        public void It_returns_all_devices()
        {
            // Arrange
            var xml = SerializeSampleXml(SampleDevices);
            FileStoreMock.Setup(f => f.CreateXmlReader(ExpectedSavePath))
                         .Returns(XmlReader.Create(new StringReader(xml)));

            // Act
            var result = UnderTest.GetAllDevices();

            // Assert
            Assert.Single(result);
            var device = result.First();
            Assert.Equal("Device1", device.Name);
            Assert.Equal(IPAddress.Parse("127.0.0.1"), device.IpAddress);
            Assert.Equal(PhysicalAddress.Parse("00-11-22-33-44-55"), device.MacAddress);
        }
    }

    public sealed class When_saving_devices : DefaultDevicesDatabaseServiceTests
    {
        [Fact]
        public void It_writes_expected_xml()
        {
            // Arrange
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var writer = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true });
            FileStoreMock.Setup(f => f.CreateXmlWriter(ExpectedSavePath, It.IsAny<XmlWriterSettings>()))
                         .Returns(writer);

            // Act
            UnderTest.SaveDevices(SampleDevices);

            // Ensure writer was flushed/disposed by the method
            var written = sb.ToString();

            // Assert
            Assert.Contains("<NetworkDetails", written);
            Assert.Contains("<Name>Device1</Name>", written);
            Assert.Contains(IPAddress.Loopback.ToString(), written);
            Assert.Contains("001122334455", written);
            FileStoreMock.Verify(f => f.CreateXmlWriter(ExpectedSavePath, It.IsAny<XmlWriterSettings>()), Times.Once);
        }
    }

    public sealed class When_load_fails : DefaultDevicesDatabaseServiceTests
    {
        [Fact]
        public void It_deletes_file_and_propagates_exception()
        {
            // Arrange
            FileStoreMock.Setup(f => f.CreateXmlReader(ExpectedSavePath)).Throws(new InvalidDataException("bad xml"));

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => UnderTest.GetAllDevices());
            FileStoreMock.Verify(f => f.Delete(ExpectedSavePath), Times.Once);
        }
    }
}
