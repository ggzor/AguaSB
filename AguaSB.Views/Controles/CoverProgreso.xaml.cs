using System;
using System.Windows;
using System.Windows.Controls;

namespace AguaSB.Views.Controles
{
    public partial class CoverProgreso : UserControl
    {
        public CoverProgreso()
        {
            InitializeComponent();
        }

        public string Texto
        {
            get { return (string)GetValue(TextoProperty); }
            set { SetValue(TextoProperty, value); }
        }

        public bool Visible
        {
            get { return (bool)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        public static readonly DependencyProperty TextoProperty =
            DependencyProperty.Register(nameof(Texto), typeof(string), typeof(CoverProgreso), new PropertyMetadata("Cargando..."));

        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register(nameof(Visible), typeof(bool), typeof(CoverProgreso), new PropertyMetadata(false));
    }
}
