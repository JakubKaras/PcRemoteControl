using Microsoft.Extensions.Logging;
using NetworkCommunicator;
using PcRemoteControl.Models;

namespace PcRemoteControl
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                // Dependency Injections
                .Services
                    .AddSingleton<MainViewModel>()
                    .AddSingleton<MainPage>()
                    .InstallNetworkCommunicator(FileSystem.AppDataDirectory);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
