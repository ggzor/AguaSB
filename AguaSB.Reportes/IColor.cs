namespace AguaSB.Reportes
{
    public interface IColor
    {
        RGB this[int x, int y] { get; set; }

        RGB this[int xi, int yi, int xf, int yf] { get; set; }
    }
}
