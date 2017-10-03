using System;
using System.Windows;

using MahApps.Metro.Controls;

namespace AguaSB
{
    public partial class VentanaPrincipal : MetroWindow
    {
        public VentanaPrincipalViewModel ViewModel { get; }

        public VentanaPrincipal(VentanaPrincipalViewModel viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
        }

        #region Propiedades
        public double AnchoExtensionView
        {
            get { return (double)GetValue(AnchoExtensionViewProperty); }
            set { SetValue(AnchoExtensionViewProperty, value); }
        }
        #endregion

        private const double AnchoMinimoPorView = 400.0;
        public const double Margen = 20.0;
        public const double AnchoRequerido = AnchoMinimoPorView + (2 * Margen);

        private void AjustarExtensionViews(object sender, SizeChangedEventArgs e)
        {
            var anchoContenedor = e.NewSize.Width;

            if (anchoContenedor < AnchoRequerido)
            {
                // Sólo cabe un view, lo compactamos de acuerdo al contenedor;
                AnchoExtensionView = anchoContenedor - (2 * Margen);
            }
            else
            {
                var cantidadDeViews = (int)(anchoContenedor / AnchoRequerido);

                var espacioPorView = anchoContenedor / cantidadDeViews;

                AnchoExtensionView = espacioPorView - (2 * Margen);
            }
        }

        #region DP´s
        public static readonly DependencyProperty AnchoExtensionViewProperty =
            DependencyProperty.Register(nameof(AnchoExtensionView), typeof(double), typeof(VentanaPrincipal), new PropertyMetadata(400.0));
        #endregion
    }
}
