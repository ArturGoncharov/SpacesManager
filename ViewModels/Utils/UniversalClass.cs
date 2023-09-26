using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Eneca.SpacesManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneca.SpacesManager.ViewModels.Utils;
public static class UniversalClass
{
    /// <summary>
    /// Получает имя основного варианта проектирования из заданного документа.
    /// </summary>
    /// <param name="linkDoc">Документ Revit, из которого нужно получить основной вариант проектирования.</param>
    /// <returns>Имя основного варианта проектирования или null, если основной вариант не найден.</returns>
    public static string GetPrimaryDesignOption(Document doc) 
    {
        List<DesignOption> link_DesignOptions = new FilteredElementCollector(doc).OfClass(typeof(DesignOption)).Cast<DesignOption>().ToList();
        string designOptionName = null;
        foreach (DesignOption designOption in link_DesignOptions)
        {
            if(designOption.IsPrimary==true)
            {
                designOptionName=designOption.Name;
            }
        }
        return designOptionName;
    }   

    public static List<Room> GetRoomsWithPrimaryDesignOption(List<Room> rooms, string nameDesignOption) 
    {
        List<Room> needRooms=new();
        foreach (var room in rooms)
        {
            if (room.DesignOption == null)
            {
                needRooms.Add(room);
            }
            else
            {
                if (room.DesignOption.Name == nameDesignOption)
                {
                    needRooms.Add(room);
                }
            }
        }
        return needRooms;
    }   

    /// <summary>
    /// Получает экземпляр связанного файла Revit по указанному индексу.
    /// </summary>
    /// <param name="index">Индекс для поиска связанного файла.</param>
    /// <returns>Экземпляр связанного файла Revit или null, если файл не найден.</returns>
    public static RevitLinkInstance GetLinkFile(string index)
    {
        Document doc = RevitApi.Document;
        var link_Docs = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).ToElements();
        foreach (var link_Doc in link_Docs)
        {
            if (link_Doc.Name.Contains(index))
                return link_Doc as RevitLinkInstance;
        }

        return null;
    }

    /// <summary>
    /// Получает список уникальных имен уровней, на которых расположены комнаты в связанном файле Revit по указанному индексу.
    /// </summary>
    /// <param name="index">Индекс для поиска связанного файла.</param>
    /// <param name="doc">Документ Revit, в котором выполняется поиск комнат.</param>
    /// <returns>Список уникальных имен уровней, на которых расположены комнаты, или пустой список, если комнаты не найдены.</returns>
    public static List<string> GetLevelsNameWithRoom(string index, Document doc)
    {
        List<string> levelsName = new();
            
        var linkIntance = GetLinkFile(index);
        var linkDoc = linkIntance.GetLinkDocument();
        List<Room> roomsLinkFile = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Rooms).Where(e => e is Room && e.Location != null).Cast<Room>().ToList();
        foreach (var room in roomsLinkFile) 
        { 
            levelsName.Add(room.Level.Name);

        }
        List<string> uniqueListLevelsName = levelsName.Distinct().ToList();

        return uniqueListLevelsName;
    }


    public static List<Space> CheckPointInSpace(List<SpatialElement> oldSpaces, XYZ point )
    {
        Document doc = RevitApi.Document;

        List<Space> checkList = new();
        foreach (Space space in oldSpaces)
        {

            if (space.IsPointInSpace(point) == true)
            {
                checkList.Add(space);
            }
            else
            {

            }

        }
        return checkList;
    }
}
