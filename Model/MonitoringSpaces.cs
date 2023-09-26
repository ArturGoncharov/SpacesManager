using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eneca.SpacesManager.ViewModels.Utils;

namespace Eneca.SpacesManager.Model
{
    /// <summary>
    /// Проверяет пространства из комнат.
    /// </summary>
    /// <param name="SelectedComboBoxItem">Выбранный элемент комбо-бокса.</param>
    /// <param name="dataItems">Коллекция элементов данных.</param>
    /// <returns>Результат мониторинга пространств.</returns>
    public class MonitoringSpaces
    {
        public MonitoringSpacesResult CheckSpacesFromRooms(string SelectedComboBoxItem, ObservableCollection<DataItem> dataItems)
        {
            MonitoringSpacesResult result = new MonitoringSpacesResult();

            result.SpaceRoomDictionary = new Dictionary<SpatialElement, string>();
            result.DataItems = dataItems;

            Document doc = RevitApi.Document;

            List<SpatialElement> oldSpaces = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).WhereElementIsNotElementType().Where(e => e is SpatialElement && e.Location != null).Cast<SpatialElement>().ToList();

            result.Spaces=oldSpaces;

            var linkIntance = UniversalClass.GetLinkFile(SelectedComboBoxItem);

            var typeLinkIntance = doc.GetElement(linkIntance.GetTypeId());

            var linkDoc = linkIntance.GetLinkDocument();
            Transform transform = linkIntance.GetTransform();

            var nameDesignOption = UniversalClass.GetPrimaryDesignOption(linkDoc);

            List<Room> roomsLinkFile = new FilteredElementCollector(linkDoc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Rooms).Where(e => e is Room && e.Location != null).Cast<Room>().ToList();

            List<Room> needRoomsLinkFile = UniversalClass.GetRoomsWithPrimaryDesignOption(roomsLinkFile, nameDesignOption);

            result.Rooms=needRoomsLinkFile;

            List<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();

            foreach (var room in needRoomsLinkFile)
            {
                foreach (var level in levels)
                {
                    foreach (var item in dataItems)
                    {
                        if ((bool)item.Choice)
                        {
                            if (item.Choice == true & room.Level.Name == item.LvlLink & level.Name == item.LvlModel)
                            {
                                if (level != null)
                                {
                                    XYZ locationPoint = transform.OfPoint((room.Location as LocationPoint).Point);
                                    var needSpace = UniversalClass.CheckPointInSpace(oldSpaces, locationPoint);
                                    if (needSpace.Count != 0)
                                    {
                                        List<string> changes=new();
                                        var firstSpace = needSpace.First();

                                        if (room.Number!=firstSpace.Number)
                                        {
                                            changes.Add($"Изменился номер на {room.Number}");
                                        }
                                        if (room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString()!=firstSpace.get_Parameter(BuiltInParameter.ROOM_NAME).AsString())
                                        {
                                            changes.Add($"Название изменилось на {room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString()}");
                                        }
                                        if (Math.Round(room.Area, 1).ToString()!=Math.Round(firstSpace.Area, 1).ToString())
                                        {
                                            changes.Add($"Площадь помещения изменилась на {Math.Round(room.Area, 2)}");
                                        }
                                        string joinedChanges = string.Join(", ", changes);


                                        if (room.Number!=firstSpace.Number || room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString()!=firstSpace.get_Parameter(BuiltInParameter.ROOM_NAME).AsString() || Math.Round(room.Area, 1).ToString()!=Math.Round(firstSpace.Area, 1).ToString())

                                        {
                                            result.SpaceRoomDictionary.Add(firstSpace, joinedChanges);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var kvp in result.SpaceRoomDictionary)
            {
                var needSpace = kvp.Key;
                var room = kvp.Value;

            }

            return result;
        }
    }
}
