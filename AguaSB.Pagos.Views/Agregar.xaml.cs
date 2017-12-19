using AguaSB.Estilos;
using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.Views;
using AguaSB.Views.Utilerias;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AguaSB.Pagos.Views
{
    public partial class Agregar : UserControl, IView
    {
        public ViewModels.Agregar ViewModel { get; }

        public AjustadorTamanoObjetos Ajustador { get; }

        public Agregar(ViewModels.Agregar viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            Ajustador = new AjustadorTamanoObjetos { Margen = 8, AnchoObjetoMinimo = 300 };

            InitializeComponent();

            ViewModel.Enfocar += (_, __) =>
            {
                Busqueda.Focus();
                Deslizar.HastaArriba(Deslizador);
            };

            viewModel.UsuarioCambiado += (_, __) => Deslizar.HastaArriba(Deslizador);
            viewModel.EncontradoUsuarioUnico += (src, args) => Resultados.IsOpen = false;
            viewModel.IniciandoBusqueda += (src, args) => Resultados.IsOpen = true;

            var usuarioSeleccionadoEventos = from evento in ViewModel.ToObservableProperties()
                                             where evento.Args.PropertyName == nameof(ViewModel.UsuarioSeleccionado)
                                             where !ViewModel.UsuarioSeleccionado
                                             select Unit.Default;

            usuarioSeleccionadoEventos.Subscribe(u => Dialogo.IsOpen = false);
        }

        private async void AbrirPanelResultados(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200).ConfigureAwait(true);
            Resultados.IsOpen = true;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaResultados.SelectedItem is Usuario usuario)
            {
                var _ = ViewModel.SeleccionarUsuario(usuario);
                Resultados.IsOpen = false;
                ListaResultados.SelectedItem = null;
            }
        }

        private void MedidasReferenciaCambiadas(object sender, SizeChangedEventArgs e)
        {
            Ajustador.AnchoContenedor = ReferenciaAnchoContenedor.ActualWidth;
        }
    }
}
