using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace AguaSB.Views.Utilerias
{
    public static class Fade
    {
        public static readonly DependencyProperty HideProperty =
            DependencyProperty.RegisterAttached("Hide", typeof(bool), typeof(Fade), new PropertyMetadata(false, Hiding));

        public static bool GetHide(UIElement elem) => (bool)elem.GetValue(HideProperty);
        public static void SetHide(UIElement elem, bool value) => elem.SetValue(HideProperty, value);


        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.RegisterAttached("Show", typeof(bool), typeof(Fade), new PropertyMetadata(false, Showing));

        public static bool GetShow(UIElement elem) => (bool)elem.GetValue(ShowProperty);
        public static void SetShow(UIElement elem, bool value) => elem.SetValue(ShowProperty, value);


        public static readonly DependencyProperty HideAndShowProperty =
            DependencyProperty.RegisterAttached("HideAndShow", typeof(bool), typeof(Fade), new PropertyMetadata(false, HideOrShow));

        private static readonly Duration AnimationDuration = TimeSpan.FromMilliseconds(200);
        private static readonly IEasingFunction EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

        public static bool GetHideAndShow(UIElement elem) => (bool)elem.GetValue(HideAndShowProperty);
        public static void SetHideAndShow(UIElement elem, bool value) => elem.SetValue(HideAndShowProperty, value);


        private static void Hiding(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement elem && e.NewValue is true && elem.Visibility != Visibility.Hidden)
            {
                var animation = new DoubleAnimation(0.0, AnimationDuration);
                animation.Completed += (src, args) => elem.Visibility = Visibility.Hidden;

                elem.BeginAnimation(UIElement.OpacityProperty, animation);
            }
        }

        private static void Showing(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement elem && e.NewValue is true && elem.Visibility != Visibility.Visible)
            {
                elem.Opacity = 0.0;
                elem.Visibility = Visibility.Visible;

                var animation = new DoubleAnimation(1.0, AnimationDuration);
                elem.BeginAnimation(UIElement.OpacityProperty, animation);
            }
        }

        private static void HideOrShow(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement elem && e.NewValue is bool value)
            {
                if (value)
                {
                    SetHide(elem, false);
                    SetShow(elem, true);
                }
                else
                {
                    SetShow(elem, false);
                    SetHide(elem, true);
                }
            }
        }
    }
}
