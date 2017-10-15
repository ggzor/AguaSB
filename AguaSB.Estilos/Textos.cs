using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AguaSB.Estilos
{
    public static class Textos
    {
        public static readonly DependencyProperty SeleccionarTodoEnFocoProperty =
            DependencyProperty.RegisterAttached("SeleccionarTodoEnFoco", typeof(bool), typeof(Textos), new PropertyMetadata(false, ManejarSeleccionarTodoEnFoco));

        private static void ManejarSeleccionarTodoEnFoco(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valor = (bool)e.NewValue;

            if (valor == true && d is TextBox elem)
            {
                elem.GotKeyboardFocus += async (_, __) =>
                {
                    await Task.Delay(20);
                    elem.SelectAll();
                };
            }
        }

        public static void SetSeleccionarTodoEnFoco(UIElement elem, bool valor) => elem.SetValue(SeleccionarTodoEnFocoProperty, valor);
        public static bool GetSeleccionarTodoEnFoco(UIElement elem) => (bool)elem.GetValue(SeleccionarTodoEnFocoProperty);
    }
}
