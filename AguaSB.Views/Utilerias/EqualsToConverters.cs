namespace AguaSB.Views.Utilerias
{
    internal static class Stringify
    {
        public static string String(this object o) => o?.ToString() ?? "null";
    }

    public class EqualsToConverter : FuncValueConverter
    {
        public EqualsToConverter() : base(Convertir) { }

        private static object Convertir(object valor, object parametro) => valor.String() == parametro.String();
    }

    public class NotEqualsToConverter : FuncValueConverter
    {
        public NotEqualsToConverter() : base(Convertir)
        {
        }

        private static object Convertir(object valor, object parametro) => valor.String() == parametro.String();
    }
}
