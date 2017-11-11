using AguaSB.Estilos;
using AguaSB.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AguaSB.Usuarios.Views
{
    public partial class Editar : UserControl, IView
    {
        public ViewModels.Editar ViewModel { get; }

        public Editar(ViewModels.Editar viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            ViewModel.Enfocar += (_, __) => Enfocar();
        }

        public void Enfocar()
        {
            if (Campos.SelectedIndex == 0)
            {
                Deslizar.HastaArriba(DeslizadorPersona);
                Persona.Enfocar();
            }
            else if (Campos.SelectedIndex == 1)
            {
                Deslizar.HastaArriba(DeslizadorNegocio);
                Negocio.Enfocar();
            }
        }
    }
}
