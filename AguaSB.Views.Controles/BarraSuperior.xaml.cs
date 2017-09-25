using System.Windows;
using System.Windows.Controls;

namespace AguaSB.Views.Controles
{
    public partial class BarraSuperior : UserControl
    {
        public FrameworkElement Icono
        {
            get { return (FrameworkElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icono), typeof(FrameworkElement), typeof(BarraSuperior), new PropertyMetadata(null));


        public string Titulo
        {
            get { return (string)GetValue(TituloProperty); }
            set { SetValue(TituloProperty, value); }
        }

        public static readonly DependencyProperty TituloProperty =
            DependencyProperty.Register(nameof(Titulo), typeof(string), typeof(BarraSuperior), new PropertyMetadata("Titulo"));


        public BarraSuperior()
        {
            InitializeComponent();
        }
    }
}
