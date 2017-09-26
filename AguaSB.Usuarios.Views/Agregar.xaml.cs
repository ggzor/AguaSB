using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Lógica de interacción para Agregar.xaml
    /// </summary>
    public partial class Agregar : UserControl
    {
        public ViewModels.Agregar ViewModel { get; }

        public Agregar()
        {
            ViewModel = new ViewModels.Agregar();
            InitializeComponent();
        }
    }
}
