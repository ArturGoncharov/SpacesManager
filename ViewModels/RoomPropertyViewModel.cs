using System.ComponentModel;

namespace Eneca.SpacesManager.ViewModels;

public class RoomPropertyViewModel : INotifyPropertyChanged
{
    private string numberRoom;
    public string NumberRoom
    {
        get { return numberRoom; }
        set
        {
            if (numberRoom != value)
            {
                numberRoom = value;
                OnPropertyChanged(nameof(NumberRoom));
            }
        }
    }

    private string nameRoom;
    public string NameRoom
    {
        get { return nameRoom; }
        set
        {
            if (nameRoom != value)
            {
                nameRoom = value;
                OnPropertyChanged(nameof(NameRoom));
            }
        }
    }

    private string areaRoom;
    public string AreaRoom
    {
        get { return areaRoom; }
        set
        {
            if (areaRoom != value)
            {
                areaRoom = value;
                OnPropertyChanged(nameof(AreaRoom));
            }
        }
    }

    private string changesRoom;
    public string ChangesRoom
    {
        get { return changesRoom; }
        set
        {
            if (changesRoom != value)
            {
                changesRoom = value;
                OnPropertyChanged(nameof(ChangesRoom));
            }
        }
    }

    private string levelRoom;
    public string LevelRoom
    {
        get { return levelRoom; }
        set
        {
            if (levelRoom != value)
            {
                levelRoom = value;
                OnPropertyChanged(nameof(LevelRoom));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}