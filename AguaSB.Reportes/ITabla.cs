namespace AguaSB.Reportes
{
    public interface ITabla
    {
        object this[int x, int y] { get; set; }

        object this[int xi, int yi, int xf, int yf] { get; set; }

        IColor Color { get; }

        IFormato Formato { get; }

        IEscritorTabla Escritor { get; }

        string Nombre { get; }
    }
}
