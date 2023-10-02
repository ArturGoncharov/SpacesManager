using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using Eneca.SpacesManager.Views;
using Nice3point.Revit.Toolkit.External;

namespace Eneca.SpacesManager.Commands;
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class EntryCommand : ExternalCommand
{
    private static SpacesManagerView _view;
    public override void Execute()
    {
        RevitApi.UiApplication ??= ExternalCommandData.Application;
        Document doc = RevitApi.Document;
        List<RevitLinkInstance> linkDocs = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().ToList();
        List<string> nameLinkFile = new();
        foreach (var linkDoc in linkDocs)
        {
            var linkExternalFile = doc.GetElement(linkDoc.GetTypeId()).GetExternalFileReference().GetLinkedFileStatus();
            var nameLinkTypeFile = doc.GetElement(linkDoc.GetTypeId()).Name;

            if (linkExternalFile == LinkedFileStatus.Loaded)
            {
                nameLinkFile.Add(nameLinkTypeFile);
            }
        }
        var viewModel = new SpacesManagerViewModel( nameLinkFile );///вызвать класс внутри, хз как , убрать логику выше в отдельный метод
        _view = new SpacesManagerView(viewModel);
        _view.ShowDialog();
    }
}