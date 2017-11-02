using System;
using System.Globalization;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public class FuncValueConverter : IValueConverter
    {
        public Func<object, object> Conversor { get; }

        public FuncValueConverter(Func<object, object> conversor) =>
            Conversor = conversor ?? throw new ArgumentNullException(nameof(conversor));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Conversor(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new InvalidOperationException();
    }
}
