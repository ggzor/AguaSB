namespace AguaSB.Views.Utilerias
{
    public class IsNullConverter : FuncValueConverter
    {
        public IsNullConverter() : base(o => o == null) { }
    }

    public class IsNotNullConverter : FuncValueConverter
    {
        public IsNotNullConverter() : base(o => o != null) { }
    }
}
