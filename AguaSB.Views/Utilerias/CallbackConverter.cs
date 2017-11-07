using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public class CallbackConverter : IValueConverter
    {
        public Func<object, object, object> Callback { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Callback?.Invoke(value, parameter) ?? DependencyProperty.UnsetValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new InvalidOperationException();
    }
}
