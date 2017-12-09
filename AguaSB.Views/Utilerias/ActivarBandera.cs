using System.Windows;

namespace AguaSB.Views.Utilerias
{
    public static class ActivarBandera
    {
        public static readonly DependencyProperty BanderaProperty =
            DependencyProperty.RegisterAttached("Bandera", typeof(bool), typeof(ActivarBandera), new PropertyMetadata(false));

        public static readonly DependencyProperty ManejarEventosProperty =
            DependencyProperty.RegisterAttached("ManejarEventos", typeof(bool), typeof(ActivarBandera), new PropertyMetadata(false, Establecer));

        private static void Establecer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IInputElement elem && d is DependencyObject obj)
            {
                elem.MouseLeftButtonDown += (src, par) => obj.SetValue(BanderaProperty, true);
            }
        }

        public static void SetBandera(DependencyObject elem, bool bandera) => elem.SetValue(BanderaProperty, bandera);
        public static bool GetBandera(DependencyObject elem) => (bool)elem.GetValue(BanderaProperty);

        public static void SetManejarEventos(DependencyObject elem, bool manejarEventos) => elem.SetValue(ManejarEventosProperty, manejarEventos);
        public static bool GetManejarEventos(DependencyObject elem) => (bool)elem.GetValue(ManejarEventosProperty);
    }
}
