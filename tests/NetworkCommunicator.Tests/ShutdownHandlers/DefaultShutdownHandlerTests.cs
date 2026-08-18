using Moq;
using NetworkCommunicator.Api.Entities;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.ShutdownHandlers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkCommunicator.Tests.ShutdownHandlers
{
    public abstract class DefaultShutdownHandlerTestsBase
    {
        protected const string ShutdownMessage = "shutdown\n";
        protected NetworkDetail Device = new() { IpAddress = IPAddress.Loopback };

        protected readonly Mock<ISocket> _socketMock = new();
        protected readonly Mock<ISocketFactory> _socketFactoryMock = new();

        protected IShutdownHandler UnderTest;

        public DefaultShutdownHandlerTestsBase()
        {
            UnderTest = new DefaultShutdownHandler(_socketFactoryMock.Object);
        }
    }

    public class WhenShutdownIsSuccessful : DefaultShutdownHandlerTestsBase
    {
        private string _shutdownMessage = null!;

        public WhenShutdownIsSuccessful()
        {
            _socketMock.Setup(s => s.Send(It.IsAny<byte[]>()))
                .Callback<byte[]>(data => _shutdownMessage = Encoding.UTF8.GetString(data))
                .Returns(ShutdownMessage.Length);

            _socketFactoryMock.Setup(f => f.Create(It.IsAny<AddressFamily>(), It.IsAny<SocketType>(), It.IsAny<ProtocolType>()))
                .Returns(_socketMock.Object);
        }

        [Fact]
        public void ThenReturnsNumberOfBytesSent()
        {
            // Act
            int result = UnderTest.Shutdown(Device);
            // Assert
            Assert.Equal(9, result);
        }

        [Fact]
        public void ThenReturnsShutdownMessage()
        {
            // Act
            int result = UnderTest.Shutdown(Device);
            // Assert
            Assert.Equal(ShutdownMessage, _shutdownMessage);
        }
    }

    public class WhenCreateThrows : DefaultShutdownHandlerTestsBase
    {
        public WhenCreateThrows()
        {
            _socketFactoryMock.Setup(f => f.Create(It.IsAny<AddressFamily>(), It.IsAny<SocketType>(), It.IsAny<ProtocolType>()))
                .Throws(new Exception("create failed"));
        }

        [Fact]
        public void ThenReturnsMinusOne()
        {
            int result = UnderTest.Shutdown(Device);
            Assert.Equal(-1, result);
        }
    }

    public class WhenConnectThrows : DefaultShutdownHandlerTestsBase
    {
        public WhenConnectThrows()
        {
            _socketFactoryMock.Setup(f => f.Create(It.IsAny<AddressFamily>(), It.IsAny<SocketType>(), It.IsAny<ProtocolType>()))
                .Returns(_socketMock.Object);

            _socketMock.Setup(s => s.Connect(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Throws(new SocketException());
        }

        [Fact]
        public void ThenReturnsMinusOne()
        {
            int result = UnderTest.Shutdown(Device);
            Assert.Equal(-1, result);
        }
    }

    public class WhenSendThrows : DefaultShutdownHandlerTestsBase
    {
        public WhenSendThrows()
        {
            _socketFactoryMock.Setup(f => f.Create(It.IsAny<AddressFamily>(), It.IsAny<SocketType>(), It.IsAny<ProtocolType>()))
                .Returns(_socketMock.Object);

            _socketMock.Setup(s => s.Send(It.IsAny<byte[]>()))
                .Throws(new Exception("send failed"));
        }

        [Fact]
        public void ThenReturnsMinusOne()
        {
            int result = UnderTest.Shutdown(Device);
            Assert.Equal(-1, result);
        }
    }

    public class WhenCreatingSocket : DefaultShutdownHandlerTestsBase
    {
        public WhenCreatingSocket()
        {
            _socketFactoryMock.Setup(f => f.Create(It.IsAny<AddressFamily>(), It.IsAny<SocketType>(), It.IsAny<ProtocolType>()))
                .Returns(_socketMock.Object);
        }

        [Fact]
        public void ThenFactoryCalledWithExpectedParameters()
        {
            UnderTest.Shutdown(Device);

            _socketFactoryMock.Verify(f => f.Create(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), Times.Once);
        }
    }

    public class WhenUsingSocket : DefaultShutdownHandlerTestsBase
    {
        public WhenUsingSocket()
        {
            _socketFactoryMock.Setup(f => f.Create(It.IsAny<AddressFamily>(), It.IsAny<SocketType>(), It.IsAny<ProtocolType>()))
                .Returns(_socketMock.Object);

            _socketMock.Setup(s => s.Send(It.IsAny<byte[]>())).Returns(9);
        }

        [Fact]
        public void ThenSocketIsDisposed()
        {
            UnderTest.Shutdown(Device);

            _socketMock.Verify(s => s.Dispose(), Times.Once);
        }
    }
}
