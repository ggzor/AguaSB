namespace AguaSB.Notificaciones
{
    public abstract class Notificacion
    {
        public object Origen { get; }

        public Notificacion(object origen) => Origen = origen;
    }
}