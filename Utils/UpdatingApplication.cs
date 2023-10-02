using AutoUpdater.Models;
using System.Reflection;

namespace Eneca.SpacesManager.Utils;
/// <summary>
/// Helps application to recieve updates
/// </summary>
internal sealed class UpdatingApplication : IUpdatingApplication
{
    public string ApplicationName=>Analytics.AppName;
    public string ApplicationUpdateId => (Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute)?.Product;
    public Version CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version;
    public Uri UpdateInfoXMLLocation => new(@"https://raw.githubusercontent.com/EnecaTechnology/Updates/master/update.xml");
    public ApplicationPurposeEnum ApplicationPurpose => ApplicationPurposeEnum.Revit;
}
