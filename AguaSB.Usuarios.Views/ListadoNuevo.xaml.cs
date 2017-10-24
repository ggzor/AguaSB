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
    /// <summary>
    /// Lógica de interacción para ListadoNuevo.xaml
    /// </summary>
    public partial class ListadoNuevo : UserControl
    {
        public ListadoNuevo()
        {
            InitializeComponent();
        }

        private void MostrarFiltros(object sender, RoutedEventArgs e) => Filtros.IsOpen = true;
    }
}
