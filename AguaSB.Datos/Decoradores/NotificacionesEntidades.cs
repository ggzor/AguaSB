using AguaSB.Notificaciones;
using AguaSB.Nucleo;

namespace AguaSB.Datos.Decoradores
{
    public abstract class NotificacionEntidad<T> : Notificacion where T : IEntidad
    {
        public T Entidad { get; }

        public NotificacionEntidad(T entidad) =>
            Entidad = entidad;

        public override string Clase => "Base de datos";
    }

    public sealed class EntidadAgregada<T> : NotificacionEntidad<T> where T : IEntidad
    {
        public override string Titulo => $"Nuevo {typeof(T).Name.ToLower()}";

        public override string Descripcion => $"Se agregó \"{Entidad}\" a la base de datos.";

        public EntidadAgregada(T entidad) : base(entidad) { }
    }
}
