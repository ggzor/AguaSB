using System.Windows;

namespace AguaSB.Views.Utilerias
{
    public class DivideConverter : FuncValueConverter
    {
        public DivideConverter() : base(Dividir)
        {
        }

        private static object Dividir(object v, object p)
        {
            if (double.TryParse(v?.ToString(), out double val) && double.TryParse(p?.ToString(), out double param) && param != 0.0)
                return val / param;
            else
                return DependencyProperty.UnsetValue;
        }
    }
}
