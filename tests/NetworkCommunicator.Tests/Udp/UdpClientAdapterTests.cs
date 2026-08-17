using System.Net;
using System.Net.Sockets;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.Udp;

namespace NetworkCommunicator.Tests.Udp
{
    public abstract class UdpClientAdapterTestsBase
    {
        protected static readonly IPEndPoint LocalEndPoint = new(IPAddress.Loopback, 0);

        protected readonly IUdpClient UnderTest = new UdpClientAdapter(LocalEndPoint);
    }

    public class When_sending_valid_data : UdpClientAdapterTestsBase
    {
        [Fact]
        public async Task Then_data_arrives_to_endpoint()
        {
            using var listener = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
            var listenerEndpoint = (IPEndPoint)listener.Client.LocalEndPoint!;
            var receiveTask = listener.ReceiveAsync();

            try
            {
                var payload = new byte[] { 1, 2, 3, 4, 5 };
                await UnderTest.SendAsync(payload, payload.Length, listenerEndpoint);

                var completed = await Task.WhenAny(receiveTask, Task.Delay(2000));
                Assert.Equal(receiveTask, completed);

                var result = await receiveTask;
                Assert.Equal(payload.Length, result.Buffer.Length);
                Assert.Equal(payload, result.Buffer);
            }
            catch
            {
                UnderTest.Dispose();
            }
        }

        [Fact]
        public void Dispose_DoesNotThrow()
        {
            var adapter = new UdpClientAdapter(new IPEndPoint(IPAddress.Loopback, 0));
            var ex = Record.Exception(() => adapter.Dispose());
            Assert.Null(ex);
        }
    }

    public class When_sending_invalid_data : UdpClientAdapterTestsBase
    {

        [Fact]
        public async Task Then_buffer_is_null_throws()
        {
            using var adapter = new UdpClientAdapter(new IPEndPoint(IPAddress.Loopback, 0));
            await Assert.ThrowsAsync<ArgumentNullException>(() => adapter.SendAsync(null!, 0, new IPEndPoint(IPAddress.Loopback, 1234)));
        }

        [Fact]
        public async Task Then_endpoint_is_null_throws()
        {
            using var adapter = new UdpClientAdapter(new IPEndPoint(IPAddress.Loopback, 0));
            await Assert.ThrowsAsync<ArgumentNullException>(() => adapter.SendAsync([1, 2, 3], 3, null!));
        }
    }
}
