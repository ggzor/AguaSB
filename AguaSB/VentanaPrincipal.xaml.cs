using System;
using System.Windows;

namespace AguaSB
{
    public partial class VentanaPrincipal : Window
    {
        public VentanaPrincipalViewModel ViewModel { get; }

        public VentanaPrincipal(VentanaPrincipalViewModel viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
        }
    }
}
