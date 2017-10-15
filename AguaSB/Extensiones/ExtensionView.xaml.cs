using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AguaSB.Extensiones
{
    public partial class ExtensionView : UserControl
    {
        public ExtensionView()
        {
            InitializeComponent();
        }

        #region Propiedades
        public FrameworkElement Icono
        {
            get { return (FrameworkElement)GetValue(IconoProperty); }
            set { SetValue(IconoProperty, value); }
        }

        public Brush FondoIcono
        {
            get { return (Brush)GetValue(FondoIconoProperty); }
            set { SetValue(FondoIconoProperty, value); }
        }

        public string Titulo
        {
            get { return (string)GetValue(TituloProperty); }
            set { SetValue(TituloProperty, value); }
        }

        public string Descripcion
        {
            get { return (string)GetValue(DescripcionProperty); }
            set { SetValue(DescripcionProperty, value); }
        }

        public IEnumerable Elementos
        {
            get { return (IEnumerable)GetValue(ElementosProperty); }
            set { SetValue(ElementosProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        #endregion

        #region DP´s
        public static readonly DependencyProperty IconoProperty =
            DependencyProperty.Register(nameof(Icono), typeof(FrameworkElement), typeof(ExtensionView), new PropertyMetadata(null));

        public static readonly DependencyProperty FondoIconoProperty =
            DependencyProperty.Register(nameof(FondoIcono), typeof(Brush), typeof(ExtensionView), new PropertyMetadata(Brushes.MediumPurple));

        public static readonly DependencyProperty TituloProperty =
            DependencyProperty.Register(nameof(Titulo), typeof(string), typeof(ExtensionView), new PropertyMetadata("Título"));

        public static readonly DependencyProperty DescripcionProperty =
            DependencyProperty.Register(nameof(Descripcion), typeof(string), typeof(ExtensionView), new PropertyMetadata("Descripción"));

        public static readonly DependencyProperty ElementosProperty =
            DependencyProperty.Register(nameof(Elementos), typeof(IEnumerable), typeof(ExtensionView), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ExtensionView), new PropertyMetadata(null));
        #endregion
    }
}
