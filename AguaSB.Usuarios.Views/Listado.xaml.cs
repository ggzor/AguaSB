using System;
using System.Windows.Controls;

using AguaSB.Views;

namespace AguaSB.Usuarios.Views
{
    public partial class Listado : UserControl, IView
    {
        public ViewModels.Listado ViewModel { get; }

        public Listado(ViewModels.Listado viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(ViewModel));

            InitializeComponent();
        }
    }
}
