using Microsoft.Extensions.DependencyInjection;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.Common;
using NetworkCommunicator.DevicesDatabaseServices;
using NetworkCommunicator.PingHandlers;
using NetworkCommunicator.ShutdownHandlers;
using NetworkCommunicator.Sockets;
using NetworkCommunicator.Udp;
using NetworkCommunicator.WakeUpHandlers;
using NetworkCommunicator.Xml;

namespace NetworkCommunicator
{
    public static class NetworkCommunicatorInstaller
    {
        public static IServiceCollection InstallNetworkCommunicator(this IServiceCollection services, string appDataDirectory)
        {
            return services
                    .AddTransient<IWakeUpHandler, DefaultWakeUpHandler>()
                    .AddTransient<IShutdownHandler, DefaultShutdownHandler>()
                    .AddTransient<IPingHandler, DefaultPingHandler>()
                    .AddSingleton<INetworkInterfaceProvider, NetworkInterfaceProvider>()
                    .AddSingleton<IUdpClientFactory, UdpClientFactory>()
                    .AddTransient<IUdpClient, UdpClientAdapter>()
                    .AddSingleton<ISocketFactory, SocketFactory>()
                    .AddTransient<ISocket, SocketAdapter>()
                    .AddSingleton<IPingFactory, PingFactory>()
                    .AddTransient<IPing, PingAdapter>()
                    .AddTransient<IXmlFileStore, XmlFileStore>()
                    .AddSingleton<IDevicesDatabaseService>(provider =>
                        new DefaultDevicesDatabaseService(appDataDirectory, provider.GetRequiredService<IXmlFileStore>()));
        }
    }
}
