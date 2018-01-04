namespace AguaSB.Operaciones
{
    public interface IProveedorAmbito
    {
        IAmbito Crear();
        IAmbitoSoloLectura CrearSoloLectura();
    }
}
