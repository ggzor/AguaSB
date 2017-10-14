using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AguaSB.Views.Utilerias
{
    public class BoolOperation : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string param && (param == "&" || param == "|"))
            {
                Func<bool, bool, bool> func;
                bool neutral;
                if (param == "&")
                {
                    func = (a, b) => a && b;
                    neutral = true;
                }
                else
                {
                    func = (a, b) => a || b;
                    neutral = false;
                }

                return values.Select(v => v is true || v == null).Aggregate(neutral, func);
            }
            else
                throw new ArgumentException("The parameter is not valid. It must be either & or |", nameof(parameter));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
