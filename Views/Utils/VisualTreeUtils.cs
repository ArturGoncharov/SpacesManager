using System.Windows;
using System.Windows.Media;

namespace Eneca.SpacesManager.Views.Utils;
/// <summary>
///     Utils for wrapping Visual Tree
/// </summary>
public static class VisualTreeUtils
{
    public static IEnumerable<T> FindVisualChild<T>(DependencyObject depObj)
        where T : DependencyObject
    {
        if (depObj is null) yield break;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T dependencyObject) yield return dependencyObject;

            foreach (var childOfChild in FindVisualChild<T>(child)) yield return childOfChild;
        }
    }

    public static T FindVisualParent<T>(DependencyObject dependencyObject) where T : DependencyObject
    {
        if (dependencyObject is null) return null;
        while (true)
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            switch (parent)
            {
                case null:
                    return null;
                case T parentT:
                    return parentT;
                default:
                    dependencyObject = parent;
                    break;
            }
        }
    }

    public static void FindVisualChildrenTraversal<TSearch>(DependencyObject reference, Action<TSearch> action)
        where TSearch : class
    {
        var count = VisualTreeHelper.GetChildrenCount(reference);
        if (count == 0)
            return;

        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(reference, i);
            if (child is TSearch search)
                action(search);
            else
                FindVisualChildrenTraversal(child, action);
        }
    }
}