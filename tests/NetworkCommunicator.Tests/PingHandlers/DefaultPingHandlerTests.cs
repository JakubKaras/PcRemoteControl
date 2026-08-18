using System.Net;
using System.Net.NetworkInformation;
using Moq;
using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Enums;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.PingHandlers;

namespace NetworkCommunicator.Tests.PingHandlers
{
    public abstract class DefaultPingHandlerTestsBase
    {
        protected readonly Mock<IPingFactory> PingFactoryMock = new();
        protected readonly Mock<IPing> PingMock = new(MockBehavior.Strict);
        protected readonly Mock<IProgress<DeviceStatus>> ProgressMock = new();
        protected readonly NetworkDetail NetworkDetail;

        protected IPingHandler UnderTest;

        public DefaultPingHandlerTestsBase()
        {
            PingFactoryMock.Setup(f => f.Create()).Returns(PingMock.Object);

            NetworkDetail = new NetworkDetail { IpAddress = IPAddress.Loopback };

            UnderTest = new DefaultPingHandler(PingFactoryMock.Object);
        }
    }

    public sealed class When_ping_succeeds : DefaultPingHandlerTestsBase
    {
        [Fact]
        public async Task It_reports_loading_then_online_and_returns_online()
        {
            // Arrange
            var reported = new List<DeviceStatus>();
            ProgressMock.Setup(p => p.Report(It.IsAny<DeviceStatus>()))
                        .Callback<DeviceStatus>(s => reported.Add(s));

            // Use a real Ping to obtain a successful PingReply for loopback (deterministic)
            var realReply = await new Ping().SendPingAsync(IPAddress.Loopback);
            PingMock.Setup(p => p.SendPingAsync(It.IsAny<IPAddress>())).ReturnsAsync(realReply);
            PingMock.Setup(p => p.Dispose());

            // Act
            var result = await UnderTest.Ping(NetworkDetail, ProgressMock.Object);

            // Assert
            Assert.Equal(DeviceStatus.Online, result);
            Assert.Equal(new[] { DeviceStatus.Loading, DeviceStatus.Online }, reported);
            PingMock.Verify(p => p.Dispose(), Times.Once);
        }
    }

    public sealed class When_ping_times_out : DefaultPingHandlerTestsBase
    {
        [Fact]
        public async Task It_reports_loading_then_offline_and_returns_offline()
        {
            // Arrange
            var reported = new List<DeviceStatus>();
            ProgressMock.Setup(p => p.Report(It.IsAny<DeviceStatus>()))
                        .Callback<DeviceStatus>(s => reported.Add(s));
            PingMock.Setup(p => p.SendPingAsync(It.IsAny<IPAddress>())).ReturnsAsync((PingReply?)null);
            PingMock.Setup(p => p.Dispose());

            // Act
            var result = await UnderTest.Ping(NetworkDetail, ProgressMock.Object);

            // Assert
            Assert.Equal(DeviceStatus.Offline, result);
            Assert.Equal(new[] { DeviceStatus.Loading, DeviceStatus.Offline }, reported);
            PingMock.Verify(p => p.Dispose(), Times.Once);
        }
    }

    public sealed class When_ping_throws : DefaultPingHandlerTestsBase
    {
        [Fact]
        public async Task It_reports_loading_then_offline_and_returns_offline()
        {
            // Arrange
            var reported = new List<DeviceStatus>();
            ProgressMock.Setup(p => p.Report(It.IsAny<DeviceStatus>()))
                        .Callback<DeviceStatus>(s => reported.Add(s));

            // Simulate failure by returning null reply
            PingMock.Setup(p => p.SendPingAsync(It.IsAny<IPAddress>())).ThrowsAsync(new Exception("Ping failed"));
            PingMock.Setup(p => p.Dispose());

            // Act
            var result = await UnderTest.Ping(NetworkDetail, ProgressMock.Object);

            // Assert
            Assert.Equal(DeviceStatus.Offline, result);
            Assert.Equal(new[] { DeviceStatus.Loading, DeviceStatus.Offline }, reported);
            PingMock.Verify(p => p.Dispose(), Times.Once);
        }
    }
}
