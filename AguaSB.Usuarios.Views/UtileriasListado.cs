using System.Windows;
using System.Windows.Controls;

using AguaSB.ViewModels;

namespace AguaSB.Usuarios.Views
{
    public static class UtileriasListado
    {
        public static readonly DependencyProperty DesactivarFiltroProperty =
            DependencyProperty.RegisterAttached("DesactivarFiltro", typeof(bool), typeof(UtileriasListado), new PropertyMetadata(false, Set));

        private static void Set(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button boton && e.NewValue is true)
            {
                boton.Click += (_, __) =>
               {
                   if (boton.Tag is Activable a)
                       a.Activo = false;
               };
            }
        }

        public static void SetDesactivarFiltro(Button elem, bool val) => elem.SetValue(DesactivarFiltroProperty, val);

        public static bool GetDesactivarFiltro(Button elem) => (bool)elem.GetValue(DesactivarFiltroProperty);
    }
}
