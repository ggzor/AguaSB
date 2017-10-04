using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using MoreLinq;

using AguaSB.Controles;
using AguaSB.Views.Utilerias;

namespace AguaSB
{
    public partial class VentanaPrincipal : MetroWindow
    {
        public VentanaPrincipalViewModel ViewModel { get; }

        public VentanaPrincipal(VentanaPrincipalViewModel viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            Ajustador.AnchoObjetoMinimo = 400.0;
            Ajustador.Margen = 15.0;

            InitializeComponent();

            MapearExtensiones();
        }

        private void MapearExtensiones() => ViewModel.Extensiones.Select(ext =>
        {
            var view = new ExtensionView()
            {
                Titulo = ext.Nombre,
                Descripcion = ext.Descripcion,
                Elementos = ext.Operaciones,
                Icono = ext.Icono,
                FondoIcono = ext.Tema.BrochaSolidaWPF,
                Command = ViewModel.EjecutarOperacionComando
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
