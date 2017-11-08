using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AguaSB
{
    public static class Animaciones
    {
        public static void MostrarEnPanel(Panel panel, FrameworkElement view)
        {
            var children = panel.Children.OfType<UIElement>();

            if (children.Contains(view))
            {
                if (children.Last() == view)
                {
                    // No hacer nada. Ya esta arriba.
                }
                else
                {
                    panel.Children.Remove(view);
                    AnimarYAgregar(panel, view);
                }
            }
            else
            {
                AnimarYAgregar(panel, view);
            }
        }

        private static void AnimarYAgregar(Panel panel, FrameworkElement view)
        {
            PrepararParaEntrada(view);
            panel.Children.Add(view);
            AplicarAnimacionEntrada(view);
        }

        public static async void RemoverDeVista(Panel panel, FrameworkElement view)
        {
            var children = panel.Children.OfType<UIElement>();

            if (children.Contains(view))
            {
                if (children.Last() == view)
                {
                    PrepararParaSalida(view);
                    AplicarAnimacionSalida(view);

                    await Task.Delay(DuracionAnimaciones).ConfigureAwait(true);

                    panel.Children.Remove(view);
                }
                else
                {
                    panel.Children.Remove(view);
                }
            }
        }

        #region Animaciones
        private static void PrepararParaAnimacion(FrameworkElement view, double opacidad, double escala)
        {
            view.Opacity = opacidad;
            view.RenderTransform = new ScaleTransform(escala, escala);
            view.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private static void PrepararParaEntrada(FrameworkElement view) => PrepararParaAnimacion(view, 0.2, 0.85);
        private static void PrepararParaSalida(FrameworkElement view) => PrepararParaAnimacion(view, 1.0, 1.0);

        private static readonly TimeSpan DuracionAnimaciones = TimeSpan.FromMilliseconds(500);
        private static readonly IEasingFunction EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseOut };

        private static void AplicarAnimacion(FrameworkElement view, double escalaFinal, double opacidadFinal)
        {
            var escalarX = DoubleAnimationTo(escalaFinal);
            var escalarY = DoubleAnimationTo(escalaFinal);
            var aparecer = DoubleAnimationTo(opacidadFinal);

            escalarX.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath($"{nameof(FrameworkElement.RenderTransform)}.{nameof(ScaleTransform.ScaleX)}"));
            escalarY.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath($"{nameof(FrameworkElement.RenderTransform)}.{nameof(ScaleTransform.ScaleY)}"));
            aparecer.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath($"{nameof(FrameworkElement.Opacity)}"));

            var s = new Storyboard()
            {
                Children = { escalarX, escalarY, aparecer }
            };
            s.Begin(view);
        }

        private static DoubleAnimation DoubleAnimationTo(double to) => new DoubleAnimation(to, DuracionAnimaciones) { EasingFunction = EasingFunction };

        private static void AplicarAnimacionEntrada(FrameworkElement view) => AplicarAnimacion(view, 1.0, 1.0);

        private static void AplicarAnimacionSalida(FrameworkElement view) => AplicarAnimacion(view, 0.85, 0.0);
        #endregion      

    }
}
