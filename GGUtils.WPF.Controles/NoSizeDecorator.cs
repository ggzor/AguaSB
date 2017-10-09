using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GGUtils.WPF.Controles
{
    public class NoSizeDecorator : Decorator
    {
        private static readonly Size Zero = new Size(0, 0);

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Child.Measure(arrangeSize);
            return Zero;
        }
    }
}
