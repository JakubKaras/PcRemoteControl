using CommunityToolkit.Mvvm.ComponentModel;
using PcRemoteControl.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PcRemoteControl.Models
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<NetworkDetail> NetworkDetails { get; set; } = new ObservableCollection<NetworkDetail>();

        [ObservableProperty]
        public partial bool IsRefreshing { get; set; }

        public ICommand? RefreshCommand { get; set; }
    }
}
