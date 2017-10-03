using MahApps.Metro.Controls;
using System;
using System.Windows;

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
    }
}
