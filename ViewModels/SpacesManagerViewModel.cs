using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.Model;
using Eneca.SpacesManager.ViewModels.Utils;
using Eneca.SpacesManager.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Eneca.SpacesManager.ViewModels;
public sealed class SpacesManagerViewModel : ObservableObject, IViewModel
{
    public Action<string> ShowMessage { get; set; }

    private ICommand _monitoringSpaces;
    private ICommand _createSpaces;

    private bool _isButtonEnabled;

    /// <summary>
    /// Определяет, включена ли кнопка.
    /// </summary>
    public bool IsButtonEnabled
    {
        get { return _isButtonEnabled; }
        set
        {
            _isButtonEnabled = value;
            OnPropertyChanged(nameof(IsButtonEnabled));
        }
    }

    private List<string> _currentLvlName;
    public List<string> CurrentLvlName
    {
        get => _currentLvlName;  
        set => SetProperty(ref _currentLvlName, value);
    }

    private string _selectedComboBoxItem;
    public string SelectedComboBoxItem
    {
        get => _selectedComboBoxItem;
        set
        {
            if (_selectedComboBoxItem == value)
                return;

            _selectedComboBoxItem = value;
            OnPropertyChanged();

            IsButtonEnabled = true;

            DataItems = new ObservableCollection<DataItemViewModel>();
            var lvlnames = new List<string>();

            var levelsCurrent = LoadedLevels().CurrentFileLevels;
            var levelsLink = LoadedLevels().LinkFileLevels;

            foreach (var level in levelsCurrent)
            {
                lvlnames.Add($"{level.Name}");
            }

            CurrentLvlName = lvlnames;

            foreach (var levelCur in levelsCurrent)
            {
                foreach (var levelLink in levelsLink)
                {
                    if (Math.Round(levelCur.Elevation, 2) == Math.Round(levelLink.Elevation, 2))
                    {
                        if (!string.IsNullOrEmpty(levelCur.Name))
                        {
                            DataItems.Add(new DataItemViewModel { Choice = true, LvlLink = $"{levelLink.Name}", LvlModel = CurrentLvlName[levelsCurrent.IndexOf(levelCur)] });
                        }
                    }
                }
            }
        }
    }
    
    private ObservableCollection<DataItemViewModel> dataItems;

    /// <summary>
    /// Коллекция элементов данных.
    /// </summary>
    public ObservableCollection<DataItemViewModel> DataItems
    {
        get { return dataItems; }
        set
        {
            dataItems = value;
            OnPropertyChanged(); 
        }
    }


    private List<string> _linkFileNames;
    public List<string> LinkFileNames
    {
        get => _linkFileNames;
        set
        {
            if (_linkFileNames == value) return;
            _linkFileNames = value;
            OnPropertyChanged();
        }
    }

    private List<SpatialElement> _changedSpaces;
    public List<SpatialElement> ChangedSpaces
    {
        get => _changedSpaces;
        set
        {
            if (_changedSpaces == value) return;
            _changedSpaces = value;
            OnPropertyChanged();
        }
    }

    public SpacesManagerViewModel(List<string> linkFileNames)
    {
        LinkFileNames= linkFileNames;
    }

    public class LoadedLevelsResult
        {
            public List<Level> LinkFileLevels { get; set; }
            public List<Level> CurrentFileLevels { get; set; }
        }

    public LoadedLevelsResult LoadedLevels()
    {
        Document doc = RevitApi.Document;
        
        var linkDoc = RevitUtils.GetLinkFile(SelectedComboBoxItem).GetLinkDocument();
        List<string> listLevelName = RevitUtils.GetLevelsNameWithRoom(SelectedComboBoxItem, linkDoc);

        List<Level> lvlLinkFile = new FilteredElementCollector(linkDoc).OfClass(typeof(Level)).Cast<Level>().ToList();
        List<Level> lvlCurrentFile = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(Level)).Cast<Level>().ToList();

        List<Level> newlvlLinkFile = new();

        foreach (var lvl in lvlLinkFile) 
        {
            if (listLevelName.Contains(lvl.Name))
            {
                newlvlLinkFile.Add(lvl);
            }
            
        }
        return new LoadedLevelsResult
        {
            LinkFileLevels = newlvlLinkFile,
            CurrentFileLevels = lvlCurrentFile
        };
    }

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

    public ICommand MonitoringSpaces => _monitoringSpaces ??= new RelayCommand(() =>
    {
        MonitoringView newWindow = new();
        var monitoringViewModel = new MonitoringViewModel();
        newWindow.DataContext= monitoringViewModel;
        //newWindow.Topmost = true;

        Document doc = RevitApi.Document;
        List<SpatialElement> oldSpaces = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).WhereElementIsNotElementType().Where(e => e is SpatialElement && e.Location != null).Cast<SpatialElement>().ToList();

        SpaceMonitoringService monitoringSpaces = new ();
        var checkSpacesFromRooms = monitoringSpaces.CheckSpacesFromRooms(SelectedComboBoxItem, DataItems);
        RoomProperties = new ObservableCollection<RoomPropertyViewModel>(); // Создание ObservableCollection

        foreach (var i in checkSpacesFromRooms.SpaceRoomDictionary)
        {
            RoomProperties.Add(new RoomPropertyViewModel { NumberRoom = i.Key.Number, NameRoom = i.Key.get_Parameter(BuiltInParameter.ROOM_NAME).AsString(), AreaRoom = Math.Round(i.Key.Area, 2).ToString(), ChangesRoom = i.Value, LevelRoom = i.Key.Level.Name });
        }
        if (checkSpacesFromRooms.Spaces.Count != checkSpacesFromRooms.Rooms.Count)
        {
            ShowMessage.Invoke($"Количество Пространств {checkSpacesFromRooms.Spaces.Count} не соответсвует количеству Помещений {checkSpacesFromRooms.Rooms.Count}");
        }

        monitoringViewModel.RoomProperties = RoomProperties;
        ChangedSpaces=checkSpacesFromRooms.SpaceRoomDictionary.Keys.Cast<SpatialElement>().ToList();
        newWindow.Show();
    });

    public ICommand CreateSpaces => _createSpaces??= new RelayCommand(() =>
    {
        SpaceCreationService classInstance = new();
        classInstance.StartCommand(SelectedComboBoxItem, DataItems, ChangedSpaces);
    });

    public void OnApplicationClosing()
    {
        //throw new NotImplementedException();
    }

}