using System.ComponentModel;

namespace Eneca.SpacesManager.ViewModels;

public class DataItemViewModel : INotifyPropertyChanged
{
    private bool choice;
    public bool Choice
    {
        get { return choice; }
        set
        {
            if (choice != value)
            {
                choice = value;
                OnPropertyChanged(nameof(Choice));
            }
        }
    }

    private string lvlLink;
    public string LvlLink
    {
        get { return lvlLink; }
        set
        {
            if (lvlLink != value)
            {
                lvlLink = value;
                OnPropertyChanged(nameof(LvlLink));
            }
        }
    }

    private string lvlModel;
    public string LvlModel
    {
        get { return lvlModel; }
        set
        {
            if (lvlModel != value)
            {
                lvlModel = value;
                OnPropertyChanged(nameof(LvlModel));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}