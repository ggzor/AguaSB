using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using AguaSB.Estilos;
using AguaSB.Nucleo;
using AguaSB.Pagos.ViewModels.Dtos;
using AguaSB.Utilerias;
using AguaSB.Views;
using AguaSB.Views.Utilerias;

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
            
            viewModel.ObservableProperty(vm => vm.UsuarioSeleccionado)
                .Where(b => b == true)
                .Sample(TimeSpan.FromMilliseconds(100))
                .ObserveOnDispatcher()
                .Subscribe(o => Deslizar.HastaArriba(Deslizador));

            viewModel.ObservableProperty(vm => vm.BusquedaOpcionesUsuarios).Subscribe(ObservarBuscador);
            viewModel.ObservableProperty(vm => vm.UsuarioSeleccionado).Subscribe(seleccionado =>
            {
                if (seleccionado)
                {
                    Resultados.IsOpen = false;
                }
                else
                {
                    Dialogo.IsOpen = false;
                }
            });

            viewModel.ControladorCubierta.ObservableProperty(c => c.MostrarCubierta).Where(b => b).Subscribe(u =>
            {
                Resultados.IsOpen = false;
                Dialogo.IsOpen = false;
            });
        }

        private IDisposable Anterior;
        private void ObservarBuscador(Busqueda<ResultadoBusquedaUsuariosConContrato> busqueda)
        {
            Anterior?.Dispose();

            var cambios = from cambio in busqueda.ObservableProperty(b => b.Buscando)
                          where cambio == true
                          select Unit.Default;

            Anterior = cambios.Subscribe(u => Resultados.IsOpen = true);
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
