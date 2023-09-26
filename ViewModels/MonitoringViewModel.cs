using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Eneca.SpacesManager.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Eneca.SpacesManager.ViewModels;
public sealed class MonitoringViewModel : ObservableValidator, IViewModel
{
    public Action<string> ShowMessage { get; set; }


    private ObservableCollection<RoomPropertyView> roomPropertiesView;
    public ObservableCollection<RoomPropertyView> RoomPropertiesView
    {
        get { return roomPropertiesView; }
        set
        {
            roomPropertiesView = value;
            OnPropertyChanged();
        }
    }

    public MonitoringViewModel()
    {

    }

    public void OnApplicationClosing()
    {
        //throw new NotImplementedException();
    }
}
