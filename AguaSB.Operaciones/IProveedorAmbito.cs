namespace AguaSB.Operaciones
{
    public interface IProveedorAmbito
    {
        IAmbito Crear();
        IAmbito CrearConTransaccion();
        IAmbitoSoloLectura CrearSoloLectura();
    }
}
