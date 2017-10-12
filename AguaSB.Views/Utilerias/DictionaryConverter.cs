using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public class DictionaryConverter : IValueConverter
    {
        public IDictionary Dictionary { get; set; }

        public bool ThrowIfNotKeyFound { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Dictionary == null)
                throw new ArgumentNullException(nameof(Dictionary));

            if (Dictionary.Contains(value))
                return Dictionary[value];
            else if (ThrowIfNotKeyFound)
                throw new KeyNotFoundException($"The key {value} is not in the dictionary.");
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
