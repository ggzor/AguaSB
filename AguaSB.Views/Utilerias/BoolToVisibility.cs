using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool?)(value);

            switch ((string)parameter)
            {
                case "H":
                    if (val == true)
                        return Visibility.Visible;
                    else
                        return Visibility.Hidden;

                case "HI":
                case "IH":
                    if (val == true)
                        return Visibility.Hidden;
                    else
                        return Visibility.Visible;

                case "I":
                    if (val == true)
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;

                case null:
                case "C":
                    if (val == true)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
            }

            throw new ArgumentException("The supplied parameter is not a valid parameter.", nameof(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException();
    }
}
