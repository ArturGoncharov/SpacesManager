using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneca.SpacesManager.ViewModels
{

    public class MonitoringSpacesResult
    {
        public Dictionary<SpatialElement, string> SpaceRoomDictionary { get; set; }
        public ObservableCollection<DataItem> DataItems { get; set; }

        public List<Room> Rooms { get; set; }
        public List<SpatialElement> Spaces { get; set; }

    }


}

