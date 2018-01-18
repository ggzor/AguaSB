namespace AguaSB.Views.Utilerias
{
    public class IsZeroConverter : FuncValueConverter
    {
        public IsZeroConverter() : base(o => (o as decimal?) == 0.0M)
        {
        }
    }

    public class IsNotZeroConverter : FuncValueConverter
    {
        public IsNotZeroConverter() : base(o => ((o as decimal?) ?? -1.0M) != 0.0M)
        {
        }
    }
}
