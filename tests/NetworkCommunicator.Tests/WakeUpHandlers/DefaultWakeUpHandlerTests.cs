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
        protected readonly Mock<IUdpClientFactory> _udpClientFactoryMock = new();

        protected IWakeUpHandler UnderTest { get; set; } = null!;

        protected readonly NetworkDetail NetworkDetail = new()
        {
            MacAddress = PhysicalAddress.Parse("00-14-22-01-23-45"),
        };
    }

    public sealed class WhenWakeUpCallIsRequested : DefaultWakeUpHandlerTestsBase
    {
        private readonly Mock<IUdpClient> _client = new();
        private byte[] _sentPacket = null!;
        private int _sentLength = 0;
        private IPEndPoint _sentEndpoint = null!;

        public WhenWakeUpCallIsRequested() : base()
        {
            _client.Setup(client => client.SendAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<IPEndPoint>()))
                .Callback<byte[], int, IPEndPoint>((buffer, length, endpoint) =>
                {
                    _sentPacket = buffer;
                    _sentLength = length;
                    _sentEndpoint = endpoint;
                })
                .Returns(Task.CompletedTask);
            _udpClientFactoryMock.Setup(factory => factory.Create()).Returns(_client.Object);

            UnderTest = new DefaultWakeUpHandler(_udpClientFactoryMock.Object);
        }

        [Fact]
        public async Task Wakeup_request_is_sent()
        {
            // Ask for a wake up
            await UnderTest.WakeUp(NetworkDetail);

            // Request was sent
            _udpClientFactoryMock.Verify(factory => factory.Create(), Times.Once);
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
            Assert.Equal(new(IPAddress.Broadcast, 7), _sentEndpoint); // Sent to the broadcast address and port 7
        }
    }
}
