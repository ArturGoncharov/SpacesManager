using Eneca.SpacesManager.ViewModels.Utils;
using System.Windows;

namespace Eneca.SpacesManager.Views;
public partial class SpacesManagerView
{
    readonly IViewModel _viewModel;
    public SpacesManagerView(IViewModel viewModel) : this()
    {
        _viewModel = viewModel;
        viewModel.ShowMessage += ShowMessage;
        Closing += Window_Closing;
        DataContext = viewModel;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _viewModel.OnApplicationClosing();
    }

    private void ShowMessage(string text)
    {
        MessageBox.Show(this, text, "Внимание");
    }

    public SpacesManagerView()
    {
        InitializeComponent();
    }
}