using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AguaSB.Usuarios.Views.Controles
{
    public partial class ProgresoConBotones : UserControl
    {

        public bool MostrarProgreso
        {
            get { return (bool)GetValue(MostrarProgresoProperty); }
            set { SetValue(MostrarProgresoProperty, value); }
        }

        public string TextoProgreso
        {
            get { return (string)GetValue(TextoProgresoProperty); }
            set { SetValue(TextoProgresoProperty, value); }
        }

        public string TextoBotonPrimario
        {
            get { return (string)GetValue(TextoBotonPrimarioProperty); }
            set { SetValue(TextoBotonPrimarioProperty, value); }
        }

        public string TextoBotonSecundario
        {
            get { return (string)GetValue(TextoBotonSecundarioProperty); }
            set { SetValue(TextoBotonSecundarioProperty, value); }
        }

        public ICommand ComandoBotonPrimario
        {
            get { return (ICommand)GetValue(ComandoBotonPrimarioProperty); }
            set { SetValue(ComandoBotonPrimarioProperty, value); }
        }

        public ICommand ComandoBotonSecundario
        {
            get { return (ICommand)GetValue(ComandoBotonSecundarioProperty); }
            set { SetValue(ComandoBotonSecundarioProperty, value); }
        }


        public ProgresoConBotones()
        {
            InitializeComponent();
        }

        #region DP´s
        public static readonly DependencyProperty MostrarProgresoProperty =
            DependencyProperty.Register(nameof(MostrarProgreso), typeof(bool), typeof(ProgresoConBotones), new PropertyMetadata(false));

        public static readonly DependencyProperty TextoProgresoProperty =
            DependencyProperty.Register(nameof(TextoProgreso), typeof(string), typeof(ProgresoConBotones), new PropertyMetadata("Cargando..."));

        public static readonly DependencyProperty TextoBotonPrimarioProperty =
            DependencyProperty.Register(nameof(TextoBotonPrimario), typeof(string), typeof(ProgresoConBotones), new PropertyMetadata("Aceptar"));

        public static readonly DependencyProperty TextoBotonSecundarioProperty =
            DependencyProperty.Register(nameof(TextoBotonSecundario), typeof(string), typeof(ProgresoConBotones), new PropertyMetadata("Cancelar"));

        public static readonly DependencyProperty ComandoBotonPrimarioProperty =
            DependencyProperty.Register(nameof(ComandoBotonPrimario), typeof(ICommand), typeof(ProgresoConBotones), new PropertyMetadata(null));

        public static readonly DependencyProperty ComandoBotonSecundarioProperty =
            DependencyProperty.Register(nameof(ComandoBotonSecundario), typeof(ICommand), typeof(ProgresoConBotones), new PropertyMetadata(null));
        #endregion
    }
}
