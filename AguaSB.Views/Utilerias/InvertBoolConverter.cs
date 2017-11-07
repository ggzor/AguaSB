namespace AguaSB.Views.Utilerias
{
    public class InvertBoolConverter : FuncValueConverter
    {
        public InvertBoolConverter() : base(VerificarValor) { }

        private static object VerificarValor(object arg) => !((arg as bool?) == true);
    }
}
