using System.Windows;
using System.Windows.Controls;

using AguaSB.Estilos;
using AguaSB.Nucleo;

namespace AguaSB.Usuarios.Views.Controles
{
    public partial class NegocioView : UserControl, IEnfocable
    {
        public NegocioView()
        {
            InitializeComponent();
        }

        public void Enfocar() => Nombre.Focus();

        public Negocio Negocio
        {
            get { return (Negocio)GetValue(NegocioProperty); }
            set { SetValue(NegocioProperty, value); }
        }

        public static readonly DependencyProperty NegocioProperty =
            DependencyProperty.Register(nameof(Negocio), typeof(Negocio), typeof(NegocioView), new PropertyMetadata(new Negocio()));
    }
}
