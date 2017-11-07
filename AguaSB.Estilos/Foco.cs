using System.Windows;
using System.Windows.Input;

namespace AguaSB.Estilos
{
    public static class Foco
    {
        public static readonly DependencyProperty EnterAvanzaFocoProperty =
            DependencyProperty.RegisterAttached("EnterAvanzaFoco", typeof(bool), typeof(Foco), new PropertyMetadata(false, ManejarEnterAvanzaFoco));

        public static void SetEnterAvanzaFoco(UIElement elem, bool valor) => elem.SetValue(EnterAvanzaFocoProperty, valor);
        public static bool GetEnterAvanzaFoco(UIElement elem) => (bool)elem.GetValue(EnterAvanzaFocoProperty);

        public static readonly DependencyProperty SiguienteFocoProperty =
            DependencyProperty.RegisterAttached("SiguienteFoco", typeof(UIElement), typeof(Foco), new PropertyMetadata(null));

        public static void SetSiguienteFoco(UIElement elem, UIElement valor) => elem.SetValue(SiguienteFocoProperty, valor);
        public static UIElement GetSiguienteFoco(UIElement elem) => (UIElement)elem.GetValue(SiguienteFocoProperty);

        private static void ManejarEnterAvanzaFoco(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valor = (bool)e.NewValue;

            if (valor && d is UIElement elem)
            {
                elem.PreviewKeyDown += (src, args) =>
                {
                    if (args.Key == Key.Enter)
                    {
                        EnfocarSiguiente(elem);
                    }
                };
            }
        }

        public static void EnfocarSiguiente(UIElement elem)
        {
            var siguiente = GetSiguienteFoco(elem);

            if (siguiente != null)
            {
                if (siguiente is IEnfocable enfocable)
                    enfocable.Enfocar();
                else
                    siguiente.Focus();
            }
            else
            {
                var request = new TraversalRequest(FocusNavigationDirection.Next);
                elem.MoveFocus(request);
            }
        }
    }
}
