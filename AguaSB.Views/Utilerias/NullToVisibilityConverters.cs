namespace AguaSB.Views.Utilerias
{
    public class IsNullToVisibilityConverter : FuncValueConverter
    {
        private static readonly BoolToVisibility B2V = new BoolToVisibility();

        public IsNullToVisibilityConverter() : base(o => B2V.Convert(o == null, null, null, null))
        {
        }
    }

    public class IsNotNullToVisibilityConverter : FuncValueConverter
    {
        private static readonly BoolToVisibility B2V = new BoolToVisibility();

        public IsNotNullToVisibilityConverter() : base(o => B2V.Convert(o != null, null, null, null))
        {
        }
    }
}
