using CommunityToolkit.Mvvm.ComponentModel;
using Eneca.SpacesManager.ViewModels.Utils;
using System.Collections.ObjectModel;

namespace Eneca.SpacesManager.ViewModels;
public sealed class MonitoringViewModel : ObservableValidator, IViewModel
{
    public Action<string> ShowMessage { get; set; }

    private ObservableCollection<RoomPropertyViewModel> _roomProperties;
    public ObservableCollection<RoomPropertyViewModel> RoomProperties
    {
        get { return _roomProperties; }
        set
        {
            _roomProperties = value;
            OnPropertyChanged();
        }
    }

    public void OnApplicationClosing()
    {
        //throw new NotImplementedException();
    }
}
