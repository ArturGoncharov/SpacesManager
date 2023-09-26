namespace Eneca.SpacesManager.ViewModels.Utils;
public interface IViewModel
{
    Action<string> ShowMessage { get; set; }
    void OnApplicationClosing();
}
