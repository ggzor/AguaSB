using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Waf.Applications;

using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using MoreLinq;

using AguaSB.Controles;
using AguaSB.Extensiones;
using AguaSB.Notificaciones;
using AguaSB.Views.Utilerias;
using System.Reactive.Linq;

namespace AguaSB
{
    public partial class VentanaPrincipal : MetroWindow, IVentanaPrincipal
    {
        public VentanaPrincipalViewModel ViewModel { get; }

        public VentanaPrincipal(VentanaPrincipalViewModel viewModel,
            ITransformadorNotificaciones transformador, IManejadorNotificaciones manejadorNotificaciones)
        {
            if (manejadorNotificaciones == null)
                throw new ArgumentNullException(nameof(manejadorNotificaciones));

            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            Ajustador.AnchoObjetoMinimo = 400.0;
            Ajustador.Margen = 15.0;

            EjecutarOperacionComando = new DelegateCommand(EjecutarOperacion);

            InitializeComponent();

            MapearExtensiones();

            manejadorNotificaciones.Notificaciones
                .Select(transformador.Transformar)
                .Subscribe(Notificaciones.AgregarNotificacion);

            administrador = new AdministradorViews(Vista);
        }

        public void Mostrar() => ShowDialog();

        private void MapearExtensiones() => ViewModel.Extensiones.Select(ext =>
        {
            var view = new ExtensionView()
            {
                Titulo = ext.Nombre,
                Descripcion = ext.Descripcion,
                Elementos = ext.Operaciones,
                Icono = ext.Icono,
                FondoIcono = ext.Tema.BrochaSolidaWPF,
                Command = EjecutarOperacionComando
            };

            var icono = ext.Icono;

            if (icono is PackIconModern i)
            {
                i.Foreground = Brushes.White;
                i.Style = (Style)FindResource("PackIconModernStroked");
            }

            icono.Width = 80;
            icono.Height = 80;

            return view;
        }).ForEach(ext => Extensiones.Children.Add(ext));

        public ICommand EjecutarOperacionComando { get; }

        private AdministradorViews administrador;

        private void EjecutarOperacion(object param)
        {
            if (param is Operacion operacion)
            {
                Atras.Visibility = Visibility.Visible;
                administrador.TraerAlFrente(operacion.View);
            }
        }

        private void VolverAPrincipal(object sender, RoutedEventArgs e)
        {
            Atras.Visibility = Visibility.Collapsed;
            administrador.VolverAPrincipal();
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
