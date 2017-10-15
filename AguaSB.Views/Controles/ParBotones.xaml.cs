using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AguaSB.Views.Controles
{
    public partial class ParBotones : UserControl
    {
        public ParBotones()
        {
            InitializeComponent();
            Primario = primario;
            Secundario = secundario;
        }

        public Button Secundario
        {
            get { return (Button)GetValue(SecundarioProperty); }
            private set { SetValue(SecundarioPropertyKey, value); }
        }

        public Button Primario
        {
            get { return (Button)GetValue(PrimarioProperty); }
            private set { SetValue(PrimarioPropertyKey, value); }
        }

        public string TextoPrimario
        {
            get { return (string)GetValue(TextoPrimarioProperty); }
            set { SetValue(TextoPrimarioProperty, value); }
        }

        public string TextoSecundario
        {
            get { return (string)GetValue(TextoSecundarioProperty); }
            set { SetValue(TextoSecundarioProperty, value); }
        }

        public ICommand ComandoPrimario
        {
            get { return (ICommand)GetValue(ComandoPrimarioProperty); }
            set { SetValue(ComandoPrimarioProperty, value); }
        }

        public ICommand ComandoSecundario
        {
            get { return (ICommand)GetValue(ComandoSecundarioProperty); }
            set { SetValue(ComandoSecundarioProperty, value); }
        }

        #region DP´s
        private static readonly DependencyPropertyKey SecundarioPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Secundario), typeof(Button), typeof(ParBotones), new PropertyMetadata(null));

        public static readonly DependencyProperty SecundarioProperty = SecundarioPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey PrimarioPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Primario), typeof(Button), typeof(ParBotones), new PropertyMetadata(null));

        public static readonly DependencyProperty PrimarioProperty = PrimarioPropertyKey.DependencyProperty;

        public static readonly DependencyProperty TextoPrimarioProperty =
            DependencyProperty.Register(nameof(TextoPrimario), typeof(string), typeof(ParBotones), new PropertyMetadata("Aceptar"));

        public static readonly DependencyProperty TextoSecundarioProperty =
            DependencyProperty.Register(nameof(TextoSecundario), typeof(string), typeof(ParBotones), new PropertyMetadata("Cancelar"));

        public static readonly DependencyProperty ComandoPrimarioProperty =
            DependencyProperty.Register(nameof(ComandoPrimario), typeof(ICommand), typeof(ParBotones), new PropertyMetadata(null));

        public static readonly DependencyProperty ComandoSecundarioProperty =
            DependencyProperty.Register(nameof(ComandoSecundario), typeof(ICommand), typeof(ParBotones), new PropertyMetadata(null));
        #endregion
    }
}
