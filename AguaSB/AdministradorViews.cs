using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AguaSB
{
    public class AdministradorViews
    {
        public Panel Vista { get; }

        public AdministradorViews(Panel vista) =>
            Vista = vista ?? throw new ArgumentNullException(nameof(vista));

        public async void TraerAlFrente(FrameworkElement element)
        {
            await Animaciones.MostrarEnPanel(Vista, element);
        }

        public void VolverAPrincipal() => VolverAPrincipal(this, null);

        private async void VolverAPrincipal(object sender, RoutedEventArgs e)
        {
            if (Vista.Children.Count > 1 && Vista.Children.OfType<FrameworkElement>().Last() is var elem)
            {
                await Animaciones.RemoverDeVista(Vista, elem);
            }
        }
    }
}
