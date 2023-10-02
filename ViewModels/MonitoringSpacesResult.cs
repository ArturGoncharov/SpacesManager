using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;

namespace Eneca.SpacesManager.ViewModels
{
    public class MonitoringSpacesResult
    {
        public Dictionary<SpatialElement, string> SpaceRoomDictionary { get; set; }
        public ObservableCollection<DataItemViewModel> DataItems { get; set; }

        public List<Room> Rooms { get; set; }
        public List<SpatialElement> Spaces { get; set; }
    }
}

