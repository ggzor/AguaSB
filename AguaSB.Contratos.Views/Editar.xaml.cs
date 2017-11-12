using System;
using System.Windows.Controls;

using AguaSB.Estilos;
using AguaSB.Views;

namespace AguaSB.Contratos.Views
{
    public partial class Editar : UserControl, IView
    {
        public ViewModels.Editar ViewModel { get; }

        public Editar(ViewModels.Editar viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            ViewModel.Enfocar += (_, __) =>
            {
                Deslizar.HastaArriba(Deslizador);

                Contrato.Enfocar();
            };
        }
    }
}
