using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using System.Collections.ObjectModel;
using Eneca.SpacesManager.ViewModels.Utils;
using Autodesk.Revit.DB.Mechanical;

namespace Eneca.SpacesManager.Model;
/// <summary>
/// Проверяет пространства из комнат.
/// </summary>
/// <param name="SelectedComboBoxItem">Выбранный элемент комбо-бокса.</param>
/// <param name="dataItems">Коллекция элементов данных.</param>
/// <returns>Результат мониторинга пространств.</returns>
public class SpaceMonitoringService
{
    public MonitoringSpacesResult CheckSpacesFromRooms(string selectedComboBoxItem, ObservableCollection<DataItemViewModel> dataItems)
    {
        MonitoringSpacesResult result = new MonitoringSpacesResult();

        result.SpaceRoomDictionary = new Dictionary<SpatialElement, string>();
        result.DataItems = dataItems;

        Document doc = RevitApi.Document;

        List<SpatialElement> oldSpaces = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).WhereElementIsNotElementType().Where(e => e is SpatialElement && e.Location != null).Cast<SpatialElement>().ToList();

        result.Spaces=oldSpaces;

        var linkIntance = RevitUtils.GetLinkFile(selectedComboBoxItem);

        var typeLinkIntance = doc.GetElement(linkIntance.GetTypeId());

        var linkDoc = linkIntance.GetLinkDocument();
        Transform transform = linkIntance.GetTransform();

        var nameDesignOption = RevitUtils.GetPrimaryDesignOption(linkDoc);

        List<Room> roomsLinkFile = new FilteredElementCollector(linkDoc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Rooms).Where(e => e is Room && e.Location != null).Cast<Room>().ToList();

        List<Room> needRoomsLinkFile = RevitUtils.GetRoomsWithPrimaryDesignOption(roomsLinkFile, nameDesignOption);

        result.Rooms=needRoomsLinkFile;

        List<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();

        foreach (var room in needRoomsLinkFile)
        {
            foreach (var level in levels)
            {
                foreach (var item in dataItems)
                {
                    if (item.Choice)
                    {
                        if (item.Choice == true & room.Level.Name == item.LvlLink & level.Name == item.LvlModel)
                        {
                            if (level != null)
                            {
                                XYZ locationPoint = transform.OfPoint((room.Location as LocationPoint).Point);
                                var needSpace = RevitUtils.CheckPointInSpace(oldSpaces, locationPoint);
                                if (needSpace.Count != 0)
                                {
                                    List<string> changes=new();
                                    var firstSpace = needSpace.First();


                                    if (room.Number!=firstSpace.Number)
                                    {
                                        changes.Add($"Номер - {room.Number}");
                                    }
                                    if (room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString()!=firstSpace.get_Parameter(BuiltInParameter.ROOM_NAME).AsString())
                                    {
                                        changes.Add($"Название -  {room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString()}");
                                    }
                                    if (Math.Round(room.Area, 1).ToString()!=Math.Round(firstSpace.Area, 1).ToString())
                                    {
                                        changes.Add($"Площадь - {Math.Round(room.Area, 2)}");
                                    }

                                    if (firstSpace.LookupParameter("ADSK_Тип помещения")!=null & room.LookupParameter("ADSK_Тип помещения")!=null)
                                    {
                                        if ($"{firstSpace.LookupParameter("ADSK_Тип помещения").AsString()}" != $"{room.LookupParameter("ADSK_Тип помещения").AsString()}")
                                        {
                                            changes.Add($"ADSK_Тип помещения - {room.LookupParameter("ADSK_Тип помещения").AsString()}");
                                        }
                                    }
                                    if (firstSpace.LookupParameter("ADSK_Номер квартиры") != null & room.LookupParameter("ADSK_Номер квартиры") != null)
                                    {
                                        if ($"{firstSpace.LookupParameter("ADSK_Номер квартиры").AsString()}" != $"{room.LookupParameter("ADSK_Номер квартиры").AsString()}")
                                        {
                                            changes.Add($"ADSK_Номер квартиры - {room.LookupParameter("ADSK_Номер квартиры").AsString()}");
                                        }
                                    }
                                    if (firstSpace.LookupParameter("ADSK_Категория помещения") != null & room.LookupParameter("ADSK_Категория помещения") != null)
                                    {
                                        if ($"{firstSpace.LookupParameter("ADSK_Категория помещения").AsString()}" != $"{room.LookupParameter("ADSK_Категория помещения").AsString()}")
                                        {
                                            changes.Add($"ADSK_Категория помещения - {room.LookupParameter("ADSK_Категория помещения").AsString()}");
                                        }
                                    }

                                    string joinedChanges = string.Join(", ", changes);

                                    if (changes.Count!=0)

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

