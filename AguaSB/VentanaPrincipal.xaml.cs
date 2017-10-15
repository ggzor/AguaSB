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

namespace AguaSB
{
    public partial class VentanaPrincipal : MetroWindow, IVentanaPrincipal
    {
        public VentanaPrincipalViewModel ViewModel { get; }

        public ICommand EjecutarOperacionComando { get; }
        private AdministradorViews administrador;

        public VentanaPrincipal(VentanaPrincipalViewModel viewModel, ITransformadorExtensiones transformador, IObservable<NotificacionView> notificaciones)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            Ajustador.AnchoObjetoMinimo = 400.0;
            Ajustador.Margen = 15.0;

            EjecutarOperacionComando = new AsyncDelegateCommand(EjecutarOperacion);

            InitializeComponent();

            notificaciones.Subscribe(Notificaciones.AgregarNotificacion);
            TransformarExtensiones(viewModel, transformador);

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

        private async Task EjecutarOperacion(object param)
        {
            if (param is Operacion operacion)
            {
                Atras.Visibility = Visibility.Visible;

                await administrador.TraerAlFrente(operacion.Visualization);

                await operacion.ViewModel.Nodo.Entrar(null);

                // Para permitir que la lógica de foco funcione correctamente
                await Task.Delay(20);
                operacion.View.Entrar();
            }
        }

        private async void VolverAPrincipal(object sender, RoutedEventArgs e)
        {
            Atras.Visibility = Visibility.Collapsed;
            await administrador.VolverAPrincipal();
        }

        #region Ajuste de tamaños
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
