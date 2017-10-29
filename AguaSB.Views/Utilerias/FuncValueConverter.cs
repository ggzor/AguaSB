using System;
using System.Globalization;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public class FuncValueConverter : IValueConverter
    {
        public Func<object, string> Conversor { get; }

        public FuncValueConverter(Func<object, string> conversor) =>
            Conversor = conversor ?? throw new ArgumentNullException(nameof(conversor));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Conversor(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new InvalidOperationException();
    }
}
