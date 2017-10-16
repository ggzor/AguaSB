using System;
using System.Windows.Controls;

using AguaSB.Views;

namespace AguaSB.Contratos.Views
{
    public partial class Agregar : UserControl, IView
    {
        public ViewModels.Agregar ViewModel { get; }

        public Agregar(ViewModels.Agregar viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            ViewModel.Enfocar += (_, __) => TipoContrato.Focus();
        }
    }
}
