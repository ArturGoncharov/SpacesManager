using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using Eneca.SpacesManager.Views;
using Nice3point.Revit.Toolkit.External;

namespace Eneca.SpacesManager.Commands;
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class TestCommand : ExternalCommand
{
    private static SpacesManagerView _view;
    public override void Execute()
    {
        RevitApi.UiApplication ??= ExternalCommandData.Application;


        Document doc = RevitApi.Document;
        List<RevitLinkInstance> link_Docs = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().ToList();
        List<string> nameLinkFile = new List<string>();
        foreach (var link_Doc in link_Docs)
        {
            var linkExternalFile = doc.GetElement(link_Doc.GetTypeId()).GetExternalFileReference().GetLinkedFileStatus();
            var nameLinkTypeFile = doc.GetElement(link_Doc.GetTypeId()).Name;

            if (linkExternalFile == LinkedFileStatus.Loaded)
            {
                nameLinkFile.Add(nameLinkTypeFile);
            }
        }
        
        var viewModel = new SpacesManagerViewModel( nameLinkFile );
        _view = new SpacesManagerView(viewModel);
        _view.ShowDialog();
    }
}