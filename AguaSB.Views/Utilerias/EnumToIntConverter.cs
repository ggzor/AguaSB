using System.Windows;

namespace AguaSB.Views.Utilerias
{
    public class EnumToIntConverter : FuncValueConverter
    {
        public EnumToIntConverter() : base(Convertir) { }

        private static object Convertir(object arg)
        {
            try
            {
                return (int)arg;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
