using System.Windows;
using System.Windows.Controls;

namespace AguaSB.Views.Controles
{
    public partial class AnilloProgreso : UserControl
    {
        public AnilloProgreso()
        {
            InitializeComponent();
        }

        public bool EsVisible
        {
            get { return (bool)GetValue(EsVisibleProperty); }
            set { SetValue(EsVisibleProperty, value); }
        }

        public string Texto
        {
            get { return (string)GetValue(TextoProperty); }
            set { SetValue(TextoProperty, value); }
        }

        #region DP´s
        public static readonly DependencyProperty EsVisibleProperty =
            DependencyProperty.Register(nameof(EsVisible), typeof(bool), typeof(AnilloProgreso), new PropertyMetadata(false));

        public static readonly DependencyProperty TextoProperty =
            DependencyProperty.Register(nameof(Texto), typeof(string), typeof(AnilloProgreso), new PropertyMetadata(""));
        #endregion
    }
}
