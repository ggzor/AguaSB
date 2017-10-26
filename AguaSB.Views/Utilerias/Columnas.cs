using System.Windows;
using System.Windows.Controls;

namespace AguaSB.Views.Utilerias
{
    public static class Columnas
    {
        public static readonly DependencyProperty EsVisibleProperty =
            DependencyProperty.RegisterAttached("EsVisible", typeof(bool), typeof(Columnas), new PropertyMetadata(true, CambiarValor));

        private static void CambiarValor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var columna = (GridViewColumn)d;
            var visible = (bool)e.NewValue;

            if (visible)
                columna.ClearValue(FrameworkElement.WidthProperty);
            else
                columna.SetValue(FrameworkElement.WidthProperty, 0.0);
        }

        public static void SetEsVisible(GridViewColumn columna, bool valor) => columna.SetValue(EsVisibleProperty, valor);
        public static bool GetEsVisible(GridViewColumn columna) => (bool)columna.GetValue(EsVisibleProperty);
    }
}
