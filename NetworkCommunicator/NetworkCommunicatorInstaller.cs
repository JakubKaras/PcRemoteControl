using Microsoft.Extensions.DependencyInjection;
using NetworkCommunicator.Api.Interfaces;
using NetworkCommunicator.DevicesDatabaseServices;
using NetworkCommunicator.PingHandlers;
using NetworkCommunicator.ShutdownHandlers;
using NetworkCommunicator.WakeUpHandlers;

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
                    .AddSingleton<IDevicesDatabaseService>(new DefaultDevicesDatabaseService(appDataDirectory));
        }
    }
}
