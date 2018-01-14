namespace AguaSB.Views.Utilerias
{
    public class IsZero : FuncValueConverter
    {
        public IsZero() : base(o => (o as decimal?) == 0.0M)
        {
        }
    }

    public class IsNotZero : FuncValueConverter
    {
        public IsNotZero() : base(o => (o as decimal?) != 0.0M)
        {
        }
    }
}
