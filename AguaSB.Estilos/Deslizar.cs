using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AguaSB.Estilos
{
    public static class Deslizar
    {
        public static void HastaArriba(ScrollViewer deslizador)
        {
            var animacion = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(1000),
                To = 0
            };

            Aplicar(deslizador, animacion);
        }

        public static void Aplicar(ScrollViewer panel, DoubleAnimation animacion) => new Deslizador(panel, animacion);

        private class Deslizador : UIElement
        {
            public static readonly QuarticEase Balanceo = new QuarticEase { EasingMode = EasingMode.EaseOut };

            public ScrollViewer Panel { get; }

            public DoubleAnimation Animacion { get; }

            public Deslizador(ScrollViewer panel, DoubleAnimation animacion)
            {
                Panel = panel ?? throw new ArgumentNullException(nameof(panel));
                Animacion = animacion ?? throw new ArgumentNullException(nameof(animacion));

                animacion.From = panel.VerticalOffset;
                animacion.EasingFunction = Balanceo;

                BeginAnimation(DesplazamientoProperty, animacion);
            }

            public double Desplazamiento
            {
                get { return (double)GetValue(DesplazamientoProperty); }
                set { SetValue(DesplazamientoProperty, value); }
            }

            public static readonly DependencyProperty DesplazamientoProperty =
                DependencyProperty.Register(nameof(Desplazamiento), typeof(double), typeof(Deslizador), new PropertyMetadata(0.0, Cambiado));

            private static void Cambiado(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is Deslizador deslizador && e.NewValue is double desplazamiento)
                {
                    deslizador.Panel.ScrollToVerticalOffset(desplazamiento);
                }
            }
        }
    }
}
