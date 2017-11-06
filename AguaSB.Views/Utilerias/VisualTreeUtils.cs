using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AguaSB.Views.Utilerias
{
    public static class VisualTreeUtils
    {
        public static T FindVisualChild<T>(DependencyObject obj) where T : Visual =>
            Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(obj))
            .Select(i => VisualTreeHelper.GetChild(obj, i))
            .OfType<T>()
            .FirstOrDefault();
    }
}
