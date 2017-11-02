using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Waf.Applications;

using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using MoreLinq;

using AguaSB.Extensiones;
using AguaSB.Notificaciones;
using AguaSB.Views.Utilerias;
using AguaSB.Navegacion;

namespace AguaSB
{
    public partial class VentanaPrincipal : MetroWindow, IVentanaPrincipal
    {
        public VentanaPrincipalViewModel ViewModel { get; }

        public ICommand EjecutarOperacionComando { get; }
        private readonly AdministradorViews administrador;

        public VentanaPrincipal(VentanaPrincipalViewModel viewModel, ITransformadorExtensiones transformador,
            ManejadorNavegacion<Operacion> manejadorNavegacion, NavegadorDirectorio<Operacion> navegador)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            Ajustador = new AjustadorTamanoObjetos
            {
                AnchoObjetoMinimo = 400.0,
                Margen = 15.0
            };

            EjecutarOperacionComando = new AsyncDelegateCommand(EjecutarOperacion);

            InitializeComponent();

            manejadorNavegacion.Navegar = Navegar;
            manejadorNavegacion.EnDireccionNoEncontrada = EnDireccionNoEncontrada;

            TransformarExtensiones(viewModel, transformador);
            RegistrarNavegacionDePaginas(navegador);

            administrador = new AdministradorViews(Vista);
        }
        
        void IVentanaPrincipal.Mostrar() => ShowDialog();

        private void TransformarExtensiones(VentanaPrincipalViewModel viewModel, ITransformadorExtensiones transformador)
        {
            viewModel.Extensiones.Select(transformador.Transformar).Select(extension =>
            {
                extension.Command = EjecutarOperacionComando;

                if (extension.Icono is PackIconModern icono)
                    icono.Style = (Style)FindResource("PackIconModernStroked");

                return extension;
            }).ForEach(_ => Extensiones.Children.Add(_));
        }
        
        private void RegistrarNavegacionDePaginas(NavegadorDirectorio<Operacion> navegador)
        {
            foreach (var extension in ViewModel.Extensiones)
            {
                foreach (var operacion in extension.Operaciones)
                {
                    navegador.Directorio.Add($"{extension.Nombre}/{operacion.View.GetType().Name}", operacion);
                }
            }
        }

        private async Task EjecutarOperacion(object param)
        {
            if (param is Operacion operacion)
                await Navegar(operacion, null).ConfigureAwait(false);
        }

        private async void VolverAPrincipal(object sender, RoutedEventArgs e) =>
            await VolverAPrincipal().ConfigureAwait(false);

        private async Task VolverAPrincipal()
        {
            Atras.Visibility = Visibility.Collapsed;
            await administrador.VolverAPrincipal().ConfigureAwait(false);
        }

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
        public async Task Navegar(Operacion operacion, object parametro)
        {
            await VolverAPrincipal();

            Atras.Visibility = Visibility.Visible;

            await administrador.TraerAlFrente(operacion.Visualization);

            await operacion.ViewModel.Nodo.Entrar(parametro);
        }
#pragma warning restore RCS1090

        public Task EnDireccionNoEncontrada(string direccion)
        {
            // TODO: Log
            Console.WriteLine("No se encontró: " + direccion);
            return Task.CompletedTask;
        }

        #region Ajuste de tamaños de vistas de extensiones
        public AjustadorTamanoObjetos Ajustador
        {
            get { return (AjustadorTamanoObjetos)GetValue(AjustadorProperty); }
            set { SetValue(AjustadorProperty, value); }
        }

        public static readonly DependencyProperty AjustadorProperty =
            DependencyProperty.Register(nameof(Ajustador), typeof(AjustadorTamanoObjetos), typeof(VentanaPrincipal), new PropertyMetadata(new AjustadorTamanoObjetos()));

        private void AjustarAnchos(object sender, SizeChangedEventArgs e) =>
            Ajustador.AnchoContenedor = e.NewSize.Width;
        #endregion
    }
}
