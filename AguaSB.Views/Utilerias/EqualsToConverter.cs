namespace AguaSB.Views.Utilerias
{
    public class EqualsToConverter : FuncValueConverter
    {
        public EqualsToConverter() : base(Convertir) { }

        private static object Convertir(object valor, object parametro) =>
            (valor?.ToString() ?? "null") == (parametro?.ToString() ?? "null");
    }
}
