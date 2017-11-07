using System.Windows;

namespace AguaSB.Estilos
{
    public static class Icono
    {
        public static readonly DependencyProperty EnfocadoProperty =
            DependencyProperty.RegisterAttached("Enfocado", typeof(bool), typeof(Icono), new PropertyMetadata(false));

        public static void SetEnfocado(UIElement elem, bool valor) => elem.SetValue(EnfocadoProperty, valor);

        public static bool GetEnfocado(UIElement elem) => (bool)elem.GetValue(EnfocadoProperty);

        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.RegisterAttached("Error", typeof(bool), typeof(Icono), new PropertyMetadata(false));

        public static void SetError(UIElement elem, bool valor) => elem.SetValue(ErrorProperty, valor);

        public static bool GetError(UIElement elem) => (bool)elem.GetValue(ErrorProperty);
    }
}
