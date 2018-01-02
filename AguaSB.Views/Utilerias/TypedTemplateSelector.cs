using System.Windows;
using System.Windows.Controls;

namespace AguaSB.Views.Utilerias
{
    class TypedTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) =>
            (DataTemplate)(((FrameworkElement)container).FindResource(item.GetType()));
    }
}
