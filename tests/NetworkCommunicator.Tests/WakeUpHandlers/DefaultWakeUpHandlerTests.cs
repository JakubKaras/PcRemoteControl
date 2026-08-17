using Moq;
using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.WakeUpHandlers;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkCommunicator.Tests.WakeUpHandlers
{
    public abstract class DefaultWakeUpHandlerTestsBase
    {
        protected readonly Mock<INetworkInterfaceProvider> _networkInterfaceProviderMock = new();
        protected readonly Mock<IUdpClientFactory> _udpClientFactoryMock = new();

        protected IWakeUpHandler UnderTest { get; set; } = null!;

        protected readonly NetworkDetail NetworkDetail = new()
        {
            MacAddress = PhysicalAddress.Parse("00-14-22-01-23-45"),
        };
    }

    public sealed class When_No_Network_Interfaces : DefaultWakeUpHandlerTestsBase
    {
        public When_No_Network_Interfaces() : base()
        {
            _networkInterfaceProviderMock.Setup(provider => provider.GetAllNetworkInterfaces()).Returns([]);
            UnderTest = new DefaultWakeUpHandler(_networkInterfaceProviderMock.Object, _udpClientFactoryMock.Object);
        }

        [Fact]
        public async Task WakeUp_Should_Not_Send_Magic_Packet()
        {
            // Ask for a wake up
            await UnderTest.WakeUp(NetworkDetail);

            // The request is never sent, because we don't have anything to send it from
            _udpClientFactoryMock.Verify(factory => factory.Create(It.IsAny<IPEndPoint>()), Times.Never);
        }
    }

    public sealed class When_No_Active_Network_Interfaces : DefaultWakeUpHandlerTestsBase
    {
        public When_No_Active_Network_Interfaces() : base()
        {
            var downEthernetNicMock = new Mock<NetworkInterface>();
            downEthernetNicMock.Setup(n => n.OperationalStatus).Returns(OperationalStatus.Down);
            downEthernetNicMock.Setup(n => n.NetworkInterfaceType).Returns(NetworkInterfaceType.Ethernet);

            var upLoopbackNicMock = new Mock<NetworkInterface>();
            upLoopbackNicMock.Setup(n => n.OperationalStatus).Returns(OperationalStatus.Up);
            upLoopbackNicMock.Setup(n => n.NetworkInterfaceType).Returns(NetworkInterfaceType.Loopback);

            _networkInterfaceProviderMock.Setup(provider => provider.GetAllNetworkInterfaces()).Returns([downEthernetNicMock.Object, upLoopbackNicMock.Object]);
            
            UnderTest = new DefaultWakeUpHandler(_networkInterfaceProviderMock.Object, _udpClientFactoryMock.Object);
        }

        [Fact]
        public async Task WakeUp_Should_Not_Send_Magic_Packet()
        {
            // Ask for a wake up
            await UnderTest.WakeUp(NetworkDetail);

            // The request is never sent, because we don't have anything to send it from
            _udpClientFactoryMock.Verify(factory => factory.Create(It.IsAny<IPEndPoint>()), Times.Never);
        }
    }

    public sealed class When_Active_Network_Interface_Has_No_Unicast_Address : DefaultWakeUpHandlerTestsBase
    {
        public When_Active_Network_Interface_Has_No_Unicast_Address() : base()
        {
            var ipPropsMock = new Mock<IPInterfaceProperties>();
            ipPropsMock.Setup(p => p.UnicastAddresses)
                .Returns(Mock.Of<UnicastIPAddressInformationCollection>(c =>
                    c.GetEnumerator() == Enumerable.Empty<UnicastIPAddressInformation>().GetEnumerator()));

            var nicMock = new Mock<NetworkInterface>();
            nicMock.Setup(n => n.OperationalStatus).Returns(OperationalStatus.Up);
            nicMock.Setup(n => n.NetworkInterfaceType).Returns(NetworkInterfaceType.Ethernet);
            nicMock.Setup(n => n.GetIPProperties()).Returns(ipPropsMock.Object);

            _networkInterfaceProviderMock.Setup(provider => provider.GetAllNetworkInterfaces()).Returns([nicMock.Object]);

            UnderTest = new DefaultWakeUpHandler(_networkInterfaceProviderMock.Object, _udpClientFactoryMock.Object);
        }

        [Fact]
        public async Task WakeUp_Should_Not_Send_Magic_Packet()
        {
            // Ask for a wake up
            await UnderTest.WakeUp(NetworkDetail);

            // The request is never sent, because we don't have anything to send it from
            _udpClientFactoryMock.Verify(factory => factory.Create(It.IsAny<IPEndPoint>()), Times.Never);
        }
    }


    public sealed class When_Active_Network_Interface_Has_No_Unicast_InterNetwork_Address : DefaultWakeUpHandlerTestsBase
    {
        public When_Active_Network_Interface_Has_No_Unicast_InterNetwork_Address() : base()
        {
            var unicastIpAddress = Mock.Of<UnicastIPAddressInformation>(ip => ip.Address == IPAddress.IPv6Loopback);
            var unicastList = new List<UnicastIPAddressInformation> { unicastIpAddress };
            var unicastIpAddressInformationCollectionMock = new Mock<UnicastIPAddressInformationCollection>();
            unicastIpAddressInformationCollectionMock.Setup(c => c.GetEnumerator()).Returns(unicastList.GetEnumerator());

            var ipInterfacePropertiesMock = new Mock<IPInterfaceProperties>();
            ipInterfacePropertiesMock.Setup(properties => properties.UnicastAddresses).Returns(unicastIpAddressInformationCollectionMock.Object);

            var nicMock = new Mock<NetworkInterface>();
            nicMock.Setup(n => n.OperationalStatus).Returns(OperationalStatus.Up);
            nicMock.Setup(n => n.NetworkInterfaceType).Returns(NetworkInterfaceType.Ethernet);
            nicMock.Setup(n => n.GetIPProperties()).Returns(ipInterfacePropertiesMock.Object);

            _networkInterfaceProviderMock.Setup(provider => provider.GetAllNetworkInterfaces()).Returns([nicMock.Object]);

            UnderTest = new DefaultWakeUpHandler(_networkInterfaceProviderMock.Object, _udpClientFactoryMock.Object);
        }

        [Fact]
        public async Task WakeUp_Should_Not_Send_Magic_Packet()
        {
            // Ask for a wake up
            await UnderTest.WakeUp(NetworkDetail);

            // The request is never sent, because we don't have anything to send it from
            _udpClientFactoryMock.Verify(factory => factory.Create(It.IsAny<IPEndPoint>()), Times.Never);
        }
    }


    public sealed class When_Active_Network_Interface_Has_Unicast_InterNetwork_Address : DefaultWakeUpHandlerTestsBase
    {
        private readonly Mock<IUdpClient> _client = new();
        private byte[] _sentPacket = null!;
        private int _sentLength = 0;
        private IPEndPoint _sentEndpoint = null!;

        public When_Active_Network_Interface_Has_Unicast_InterNetwork_Address() : base()
        {
            var unicastIpAddress = Mock.Of<UnicastIPAddressInformation>(info => info.Address == IPAddress.Loopback); // IPv4 address
            var unicastIpAddressList = new List<UnicastIPAddressInformation> { unicastIpAddress };

            var unicastIpAddressInformationCollectionMock = new Mock<UnicastIPAddressInformationCollection>();
            unicastIpAddressInformationCollectionMock.Setup(c => c.GetEnumerator()).Returns(unicastIpAddressList.GetEnumerator());

            var ipInterfacePropertiesMock = new Mock<IPInterfaceProperties>();
            ipInterfacePropertiesMock.Setup(properties => properties.UnicastAddresses).Returns(unicastIpAddressInformationCollectionMock.Object);

            var nicMock = new Mock<NetworkInterface>();
            nicMock.Setup(n => n.OperationalStatus).Returns(OperationalStatus.Up);
            nicMock.Setup(n => n.NetworkInterfaceType).Returns(NetworkInterfaceType.Ethernet);
            nicMock.Setup(n => n.GetIPProperties()).Returns(ipInterfacePropertiesMock.Object);

            _networkInterfaceProviderMock.Setup(provider => provider.GetAllNetworkInterfaces()).Returns([nicMock.Object]);

            _client.Setup(client => client.SendAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<IPEndPoint>()))
                .Callback<byte[], int, IPEndPoint>((buffer, length, endpoint) =>
                {
                    _sentPacket = buffer;
                    _sentLength = length;
                    _sentEndpoint = endpoint;
                })
                .Returns(Task.CompletedTask);
            _udpClientFactoryMock.Setup(factory => factory.Create(It.IsAny<IPEndPoint>())).Returns(_client.Object);

            UnderTest = new DefaultWakeUpHandler(_networkInterfaceProviderMock.Object, _udpClientFactoryMock.Object);
        }

        [Fact]
        public async Task Wakeup_request_is_sent()
        {
            // Ask for a wake up
            await UnderTest.WakeUp(NetworkDetail);

            // Request was sent
            _udpClientFactoryMock.Verify(factory => factory.Create(It.IsAny<IPEndPoint>()), Times.Once);
            _client.Verify(client => client.SendAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<IPEndPoint>()), Times.Once);
        }

        [Fact]
        public async Task Request_has_correct_structure()
        {
            // Ask for a wake up
            await UnderTest.WakeUp(NetworkDetail);

            // Request structure is correct
            Assert.True(_sentPacket.Take(6).All(b => b == 0xFF)); // First 6 bytes are 0xFF
            Assert.True(_sentPacket.Skip(6).Take(16 * 6).SequenceEqual(Enumerable.Repeat(NetworkDetail.MacAddress.GetAddressBytes(), 16).SelectMany(x => x))); // Next 16 repetitions of the MAC address)
            Assert.Equal(102, _sentLength); // 6 bytes of 0xFF + 16 repetitions of 6-byte MAC address
            Assert.Equal(new(new IPAddress([224, 0, 0, 1]), 7), _sentEndpoint); // Sent to the multicast address and port 7
        }
    }
}
