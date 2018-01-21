namespace AguaSB.Views.Utilerias
{
    internal static class Util
    {
        public static readonly IsZeroConverter IsZeroConverter = new IsZeroConverter();
        public static readonly IsNotZeroConverter IsNotZeroConverter = new IsNotZeroConverter();
        public static readonly BoolToVisibility BoolToVisibility = new BoolToVisibility();
    }

    public class IsZeroToVisibilityConverter : FuncValueConverter
    {
        public IsZeroToVisibilityConverter() : base((o, p) => Util.BoolToVisibility.Convert(Util.IsZeroConverter.Convert(o, null, null, null), null, p, null))
        {
        }
    }

    public class IsNotZeroToVisibilityConverter : FuncValueConverter
    {
        public IsNotZeroToVisibilityConverter() : base((o, p) => Util.BoolToVisibility.Convert(Util.IsNotZeroConverter.Convert(o, null, null, null), null, p, null))
        {
        }
    }
}
