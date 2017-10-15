using System.Windows;
using System.Windows.Input;

namespace AguaSB.Estilos
{
    public static class Foco
    {
        public static readonly DependencyProperty EnterAvanzaFocoProperty =
            DependencyProperty.RegisterAttached("EnterAvanzaFoco", typeof(bool), typeof(Foco), new PropertyMetadata(false, ManejarEnterAvanzaFoco));

        public static readonly DependencyProperty SiguienteFocoProperty =
            DependencyProperty.RegisterAttached("SiguienteFoco", typeof(UIElement), typeof(Foco), new PropertyMetadata(null));

        private static void ManejarEnterAvanzaFoco(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valor = (bool)e.NewValue;

            if (valor == true && d is UIElement elem)
            {
                elem.PreviewKeyDown += (src, args) =>
                {
                    var siguiente = elem.GetValue(SiguienteFocoProperty) as UIElement;
                    if (args.Key == Key.Enter)
                    {
                        if (siguiente != null)
                        {
                            siguiente.Focus();
                        }
                        else
                        {
                            var request = new TraversalRequest(FocusNavigationDirection.Next);
                            elem.MoveFocus(request);
                        }
                    }
                };
            }
        }

        public static void SetEnterAvanzaFoco(UIElement elem, bool valor) => elem.SetValue(EnterAvanzaFocoProperty, valor);
        public static bool GetEnterAvanzaFoco(UIElement elem) => (bool)elem.GetValue(EnterAvanzaFocoProperty);

        public static void SetSiguienteFoco(UIElement elem, bool valor) => elem.SetValue(SiguienteFocoProperty, valor);
        public static bool GetSiguienteFoco(UIElement elem) => (bool)elem.GetValue(SiguienteFocoProperty);
    }
}
