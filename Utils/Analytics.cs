using Eneca.SpacesManager.Core;
using System.IO;

namespace Eneca.SpacesManager.Utils;
/// <summary>
/// Helps to collect analycts data from projects
/// </summary>
public static class Analytics
{
    public static string AppName { get; set; }
    public static Version Version { get; set; }
    public static string UserName { get; set; }
    private static string OpenedDocumentPath => RevitApi.Document?.PathName;
    private static string RevitVersion => RevitApi.UiApplication.Application.VersionNumber;

    public static void SaveAnalytics(string comments)
    {
        try
        {
            var time = DateTime.Now;
            const string fileName = @"R:\1 - Проекты\- Координация\- Аналитика\plugins-LOG.txt";
            var analyticsLine = $"Revit {RevitVersion}\t{AppName} v{Version}\t{time}\t{UserName}\t{Path.GetFileNameWithoutExtension(OpenedDocumentPath)}\t{comments}";
            if (File.Exists(fileName))
            {
                File.AppendAllLines(fileName, new[] { analyticsLine });
            }
        }

        catch (Exception ex)
        {
            SaveExceptionReport(ex, "Не удалось сохранить аналитику");
        }
    }

    public static void SaveExceptionReport(Exception ex, string comments)
    {
        try
        {
            var time = DateTime.Now;
            var report =
                $"Time: {time}\n" +
                $"AppName: {AppName}\n" +
                $"Version: {Version}\n" +
                $"Revit {RevitVersion}\n" +
                $"User: {UserName}\n" +
                $"Opened document: {OpenedDocumentPath}\n" +
                $"\n" +
                $"{ex.Message}\n" +
                $"{ex.GetType()}\n" +
                $"Source: {ex.Source}\n" +
                $"StackTrace\n{ex.StackTrace}\n" +
                $"\nComments: {comments}";

            var reportName = $"{AppName}-{ex.GetType()}-{time.TimeOfDay.ToString().Replace(":", ".")}.txt";
            const string reportCatalog = @"R:\1 - Проекты\- Координация\- Аналитика\ErrorLogs\";
            var fileName = Path.Combine(reportCatalog, reportName);

            File.WriteAllText(fileName, report);
        }
        catch (Exception)
        {
            //не удалось сохранить отчет
        }

    }

    public static void SaveExceptionReport(Exception ex)
    {
        SaveExceptionReport(ex, string.Empty);
    }

}
