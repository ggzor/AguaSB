using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public delegate object PostProcessCallback(object value, object parameter, object possibleValue);

    public class DictionaryConverter : IValueConverter
    {
        public IDictionary Dictionary { get; set; }

        public bool ThrowIfNotKeyFound { get; set; }

        public object Default { get; set; }

        public PostProcessCallback PostProcessCallback { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Dictionary == null)
                return DependencyProperty.UnsetValue;

            object possibleValue = null;

            if (Dictionary.Contains(value))
            {
                possibleValue = Dictionary[value];
            }
            else if (ThrowIfNotKeyFound)
            {
                throw new KeyNotFoundException($"The key {value} is not in the dictionary.");
            }
            else
            {
                if (Default != null)
                    possibleValue = Default;
                else if (Dictionary.Contains("Default"))
                    possibleValue = Dictionary["Default"];
            }

            return (PostProcessCallback != null ? PostProcessCallback(value, parameter, possibleValue) : possibleValue) ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
