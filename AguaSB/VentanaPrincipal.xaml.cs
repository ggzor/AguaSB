using System;
using System.Windows;

using MahApps.Metro.Controls;

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
