using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AguaSB.Views.Controles
{
    public partial class Cover : UserControl
    {
        public Cover()
        {
            InitializeComponent();
        }

        public UIElement Contenido
        {
            get { return (UIElement)GetValue(ContenidoProperty); }
            set { SetValue(ContenidoProperty, value); }
        }

        public bool Visible
        {
            get { return (bool)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        public static readonly DependencyProperty ContenidoProperty =
            DependencyProperty.Register(nameof(Contenido), typeof(UIElement), typeof(Cover), new PropertyMetadata(null));

        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register(nameof(Visible), typeof(bool), typeof(Cover), new PropertyMetadata(false, ManejarVisibleCambiado));

        private static async void ManejarVisibleCambiado(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
                await (d as Cover).ActualizarVista();
        }

        private const double OpacidadDestino = 0.9;
        private static readonly TimeSpan DuracionAnimacion = TimeSpan.FromMilliseconds(500);

        private async Task ActualizarVista()
        {
            DoubleAnimation animacion = new DoubleAnimation(OpacidadDestino, DuracionAnimacion);

            if (Visible == true)
            {
                progreso.Opacity = 0.0;
                Visibility = Visibility.Visible;
            }
            else
            {
                animacion.To = 0.0;
            }

            progreso.BeginAnimation(OpacityProperty, animacion);

            if (Visible == false)
            {
                await Task.Delay(DuracionAnimacion);
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
