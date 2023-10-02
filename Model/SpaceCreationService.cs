using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using Eneca.SpacesManager.ViewModels.Utils;

namespace Eneca.SpacesManager.Model;
public class SpaceCreationService
{
    private List<Space> CreateSpaceFromRoom(List<Room> rooms, Document doc, Transform transform, ObservableCollection<DataItemViewModel> dataItems)
    {
        List<Space> result = new List<Space>();
        List<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();

        List<SpatialElement> oldSpaces = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).WhereElementIsNotElementType().Where(e => e is SpatialElement && e.Location != null).Cast<SpatialElement>().ToList();

        using (Transaction t = new Transaction(doc, "Create spaces"))
        {
            t.Start();
            foreach (var room in rooms)
            {
                foreach (var level in levels)
                {
                    foreach(var item in dataItems) 
                    {
                        if (item.Choice)
                        {
                            if (item.Choice==true & room.Level.Name==item.LvlLink & level.Name==item.LvlModel)
                            {
                                if (level != null)
                                {
                                    XYZ locationPoint = transform.OfPoint((room.Location as LocationPoint).Point);
                                    if (RevitUtils.CheckPointInSpace(oldSpaces, locationPoint).Count==0)
                                    {
                                        Space space = doc.Create.NewSpace(level, new UV(locationPoint.X, locationPoint.Y));
                                        space.Name = room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
                                        space.Number = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                                        space.LimitOffset = room.LimitOffset;
                                        space.BaseOffset = room.BaseOffset;
                                        if (space.LookupParameter("ADSK_Категория помещения")!=null & room.LookupParameter("ADSK_Категория помещения")!=null)
                                        {
                                            space.LookupParameter("ADSK_Категория помещения").Set(room.LookupParameter("ADSK_Категория помещения").AsString());
                                        }
                                        if (space.LookupParameter("ADSK_Тип помещения")!=null & room.LookupParameter("ADSK_Тип помещения")!=null)
                                        {
                                            space.LookupParameter("ADSK_Тип помещения").Set(room.LookupParameter("ADSK_Тип помещения").AsString());
                                        }
                                        if (space.LookupParameter("ADSK_Номер квартиры")!=null & room.LookupParameter("ADSK_Номер квартиры")!=null)
                                        {
                                            space.LookupParameter("ADSK_Номер квартиры").Set(room.LookupParameter("ADSK_Номер квартиры").AsString());
                                        }
                                        result.Add(space);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            t.Commit();
        }
        return result;
    }

    public void StartCommand(string selectedComboBoxItem, ObservableCollection<DataItemViewModel> dataItems, List<SpatialElement> spaces)
    {
        Document doc = RevitApi.Document;

        var linkIntance = RevitUtils.GetLinkFile(selectedComboBoxItem);

        var typeLinkIntance = doc.GetElement(linkIntance.GetTypeId());

        using (Transaction t = new Transaction(doc, "Delete modified spaces"))
        {
            try
            { 
                t.Start();
                foreach(var space in spaces)
                {
                
                    doc.Delete(space.Id);
                
                }
                doc.Regenerate();
                t.Commit();
            }
            catch { }
        }

        if (typeLinkIntance.GetParameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING) != null) 
        {
            if (typeLinkIntance.GetParameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING).AsInteger() == 0)
            {
                using (Transaction t = new Transaction(doc, "Change parameter"))
                {
                    t.Start();
                    typeLinkIntance.GetParameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING).Set(1);
                    doc.Regenerate();
                    t.Commit();
                }
            }  
        
        }
        
        Transform transform = linkIntance.GetTransform();

        var linkDoc = linkIntance.GetLinkDocument();
        var nameDesignOption = RevitUtils.GetPrimaryDesignOption(linkDoc);
        List<Room> roomsLinkFile = new FilteredElementCollector(linkDoc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Rooms).Where(e => e is Room && e.Location != null).Cast<Room>().ToList();
        List<Room> needRoomsLinkFile = RevitUtils.GetRoomsWithPrimaryDesignOption(roomsLinkFile, nameDesignOption);
        var newspaces = CreateSpaceFromRoom(needRoomsLinkFile, doc, transform, dataItems);
        MessageBox.Show($"Создано {newspaces.Count} пространств, Всего помещений {needRoomsLinkFile.Count}", "Уведомление");
        
    }
}
