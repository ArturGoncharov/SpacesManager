using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Eneca.SpacesManager.ViewModels.Utils;

namespace Eneca.SpacesManager.Model;
public class CreateSpaces
{

    public List<Space> CreateSpaceFromRoom(List<Room> rooms, Document doc, Transform transform, ObservableCollection<DataItem> dataItems)
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
                        if ((bool)item.Choice)
                        {

                            if (item.Choice==true & room.Level.Name==item.LvlLink & level.Name==item.LvlModel)
                            {
                                
                                if (level != null)
                                {

                                    XYZ locationPoint = transform.OfPoint((room.Location as LocationPoint).Point);
                                    if (UniversalClass.CheckPointInSpace(oldSpaces, locationPoint).Count==0)
                                    {
                                        Space space = doc.Create.NewSpace(level, new UV(locationPoint.X, locationPoint.Y));
                                        space.Name = room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
                                        space.Number = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                                        space.LimitOffset = room.LimitOffset;
                                        space.BaseOffset = room.BaseOffset;
                                        space.GetParameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(room.GetParameter(BuiltInParameter.ROOM_LEVEL_ID).AsValueString());
                                        try
                                        {
                                            space.LookupParameter("ADSK_Тип помещения").SetValueString(room.LookupParameter("ADSK_Тип помещения").AsValueString());
                                            space.LookupParameter("ADSK_Номер квартиры").SetValueString(room.LookupParameter("ADSK_Номер квартиры").AsValueString());
                                            space.LookupParameter("ADSK_Категория помещения").SetValueString(room.LookupParameter("ADSK_Категория помещения").AsValueString());
                                        }
                                        catch { }

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

    public void StartCommand(string SelectedComboBoxItem, ObservableCollection<DataItem> dataItems, List<SpatialElement> spaces)
    {
        Document doc = RevitApi.Document;

        var linkIntance = UniversalClass.GetLinkFile(SelectedComboBoxItem);

        var typeLinkIntance = doc.GetElement(linkIntance.GetTypeId());

        using (Transaction t = new Transaction(doc, "Delete modified spaces"))
            {
                t.Start();
                if (spaces.Count!=0)
                {
                    foreach(Space space in spaces)
                    {
                        doc.Delete(space.Id);
                    }

                }
                doc.Regenerate();
                t.Commit();
            }

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
            
        Transform transform = linkIntance.GetTransform();
        if (linkIntance != null)
        {
            var linkDoc = linkIntance.GetLinkDocument();
            var nameDesignOption = UniversalClass.GetPrimaryDesignOption(linkDoc);
            List<Room> roomsLinkFile = new FilteredElementCollector(linkDoc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Rooms).Where(e => e is Room && e.Location != null).Cast<Room>().ToList();
            List<Room> needRoomsLinkFile = UniversalClass.GetRoomsWithPrimaryDesignOption(roomsLinkFile, nameDesignOption);
            var newspaces = CreateSpaceFromRoom(needRoomsLinkFile, doc, transform, dataItems);
            MessageBox.Show($"Создано {newspaces.Count} пространств, Всего помещений {needRoomsLinkFile.Count}", "Уведомление");
        }
    }
}
