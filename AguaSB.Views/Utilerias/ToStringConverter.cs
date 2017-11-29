using System;
using System.Globalization;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value?.ToString() ?? parameter;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            System.Convert.ChangeType(value, targetType);
    }
}
