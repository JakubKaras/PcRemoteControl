using NetworkCommunicator.Api.Enums;
using NetworkCommunicator.Api.Interfaces;
using PcRemoteControl.Entities;
using PcRemoteControl.Models;

namespace PcRemoteControl
{
    public partial class MainPage : ContentPage
    {
        private readonly IDevicesDatabaseService _devicesDatabaseService;
        private readonly IPingHandler _pingHandler;
        private readonly IWakeUpHandler _wakeUpHandler;
        private readonly IShutdownHandler _shutdownHandler;

        public MainPage(
            MainViewModel viewModel,
            IDevicesDatabaseService devicesDatabaseService,
            IPingHandler pingHandler,
            IWakeUpHandler wakeUpHandler,
            IShutdownHandler shutdownHandler)
        {
            _devicesDatabaseService = devicesDatabaseService;
            _pingHandler = pingHandler;
            _wakeUpHandler = wakeUpHandler;
            _shutdownHandler = shutdownHandler;

            InitializeComponent();
            viewModel.RefreshCommand = new Command(async () => await RefreshCommand());
            BindingContext = viewModel;
        }

        private async Task RefreshCommand()
        {
            ((MainViewModel)BindingContext).IsRefreshing = true;

            var tasks = ((MainViewModel)BindingContext).NetworkDetails
                .Select(PerformPing);

            ((MainViewModel)BindingContext).IsRefreshing = false;

            await Task.WhenAll(tasks);
        }

        private async Task<DeviceStatus> PerformPing(NetworkDetail device)
        {
            var progress = new Progress<DeviceStatus>(status => device.Status = status);

            var result = await _pingHandler.Ping(NetworkDetail.ToApi(device), progress);

            return result;
        }

        private async void OnWakeUpClicked(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).BindingContext is not NetworkDetail selectedItem)
                return;

            if (await DisplayAlertAsync("Wake Up Device", $"Are you sure you wish to wake up {selectedItem.Name}", "Wake Up", "Cancel"))
                await _wakeUpHandler.WakeUp(NetworkDetail.ToApi(selectedItem));
        }

        private async void OnShutdownClicked(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).BindingContext is not NetworkDetail selectedItem)
                return;

            if (await PerformPing(selectedItem) != DeviceStatus.Online)
            {
                await DisplayAlertAsync("Device Is Offline", $"{selectedItem.Name} is already offline.", "Ok");
                return;
            }

            if (await DisplayAlertAsync("Shutdown Device", $"Are you sure you wish to shutdown {selectedItem.Name}", "Shutdown", "Cancel"))
                _shutdownHandler.Shutdown(NetworkDetail.ToApi(selectedItem));
        }

        private async void OnEditSwipeItemInvoked(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).BindingContext is not NetworkDetail selectedItem)
                return;

            await Navigation.PushAsync(new AddOrEditDevicePage(new AddOrEditDeviceViewModel(selectedItem, true)));
        }

        private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            if (((SwipeItem)sender).BindingContext is not NetworkDetail selectedItem)
                return;

            if (!await DisplayAlertAsync("Delete Device", $"Are you sure you wish to remove {selectedItem.Name}", "Delete", "Cancel")){

                return;
            }

            ((MainViewModel)BindingContext).NetworkDetails.Remove(selectedItem);
            _devicesDatabaseService.SaveDevices([.. ((MainViewModel)BindingContext).NetworkDetails.Select(NetworkDetail.ToApi)]);
        }

        private async void AddDeviceBtn_Clicked(object sender, EventArgs e)
        {
            MainViewModel vm = (MainViewModel)BindingContext;

            vm.NetworkDetails!.Add(new NetworkDetail());
            await Navigation.PushAsync(new AddOrEditDevicePage(new AddOrEditDeviceViewModel(vm.NetworkDetails.Last(), false)));
        }

        private void OnAppearing(object? sender, EventArgs e)
        {
            if (((MainViewModel)BindingContext).NetworkDetails.Count == 0)
            {
                try
                {
                    _devicesDatabaseService
                        .GetAllDevices()
                        ?.ForEach(device => ((MainViewModel)BindingContext).NetworkDetails.Add(NetworkDetail.FromApi(device)));
                }
                catch
                {

                }
            }
            else
            {
                _devicesDatabaseService.SaveDevices([.. ((MainViewModel)BindingContext).NetworkDetails.Select(NetworkDetail.ToApi)]);
            }
        }
    }
}
