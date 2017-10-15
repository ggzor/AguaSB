using System;
using System.Globalization;
using System.Windows.Data;

namespace AguaSB.Conversores
{
    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value?.ToString() ?? "Vacío";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            System.Convert.ChangeType(value, targetType);
    }
}
