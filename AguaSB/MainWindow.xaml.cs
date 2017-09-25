using System.Linq;
using System.Windows;

namespace AguaSB
{
    public partial class MainWindow : IAdministradorViews
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(this);
            InitializeComponent();
        }

        public async void TraerAlFrente(FrameworkElement element)
        {
            await Animaciones.MostrarEnPanel(Vista, element);
            BotonAtras.Visibility = Visibility.Visible;
        }

        public void VolverAPrincipal() => VolverAPrincipal(this, null);

        private async void VolverAPrincipal(object sender, RoutedEventArgs e)
        {
            if (Vista.Children.Count > 1 && Vista.Children.OfType<FrameworkElement>().Last() is var elem)
            {
                await Animaciones.RemoverDeVista(Vista, elem);
                BotonAtras.Visibility = Visibility.Collapsed;
            }
        }

    }
}
