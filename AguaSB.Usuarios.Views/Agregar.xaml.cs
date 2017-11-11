using System;
using System.Threading.Tasks;
using System.Windows.Controls;

using AguaSB.Views;
using AguaSB.Estilos;

namespace AguaSB.Usuarios.Views
{
    public partial class Agregar : UserControl, IView
    {
        public ViewModels.Agregar ViewModel { get; }

        public Agregar(ViewModels.Agregar viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            TipoUsuario.SelectionChanged += async (_, __) =>
            {
                await Task.Delay(20).ConfigureAwait(true);
                Enfocar();
            };

            ViewModel.Enfocar += (_, __) => Enfocar();
        }

        public void Enfocar()
        {
            if (TipoUsuario.SelectedIndex == 0)
            {
                Deslizar.HastaArriba(DeslizadorPersona);
                Persona.Enfocar();
            }
            else if (TipoUsuario.SelectedIndex == 1)
            {
                Deslizar.HastaArriba(DeslizadorNegocio);
                Negocio.Enfocar();
            }
        }
    }
}
