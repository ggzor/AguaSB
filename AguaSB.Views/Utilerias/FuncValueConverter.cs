using System;
using System.Globalization;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public class FuncValueConverter : IValueConverter
    {
        public Func<object, object> Conversor { get; }

        public Func<object, object, object> ConversorConParametro { get; }

        public bool UsaParametro => ConversorConParametro != null;

        public FuncValueConverter(Func<object, object> conversor) =>
            Conversor = conversor ?? throw new ArgumentNullException(nameof(conversor));

        public FuncValueConverter(Func<object, object, object> conversorConParametro) =>
            ConversorConParametro = conversorConParametro ?? throw new ArgumentNullException(nameof(conversorConParametro));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            UsaParametro
            ? ConversorConParametro(value, parameter)
            : Conversor(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new InvalidOperationException();
    }
}
