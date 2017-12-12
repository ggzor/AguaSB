namespace AguaSB.Reportes
{
    public interface ILibroTablas
    {
        string Nombre { get; }

        ITabla CrearNueva(string nombre);

        void Generar();
    }
}
