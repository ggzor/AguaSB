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

        public void TraerAlFrente(FrameworkElement element) =>
            Animaciones.MostrarEnPanel(Vista, element);

        public void VolverAPrincipal()
        {
            if (Vista.Children.Count > 1 && Vista.Children.OfType<FrameworkElement>().Last() is FrameworkElement elem)
                Animaciones.RemoverDeVista(Vista, elem);
        }
    }
}
