using AguaSB.Notificaciones;
using AguaSB.Nucleo;

namespace AguaSB.Datos.Decoradores
{
    public abstract class NotificacionEntidad<T> : Notificacion where T : IEntidad
    {
        public T Entidad { get; }

        protected NotificacionEntidad(T entidad)
        {
            Entidad = entidad;
            Clase = "Base de datos";
        }
    }

    public sealed class EntidadAgregada<T> : NotificacionEntidad<T> where T : IEntidad
    {
        public EntidadAgregada(T entidad) : base(entidad)
        {
            Titulo = $"Nuevo {typeof(T).Name.ToLower()}";
            Descripcion = $"Se agregó \"{Entidad}\" a la base de datos.";
        }
    }

    public sealed class EntidadActualizada<T> : NotificacionEntidad<T> where T : IEntidad
    {
        public EntidadActualizada(T entidad) : base(entidad)
        {
            Titulo = $"Actualizando {typeof(T).Name.ToLower()}";
            Descripcion = $"Se actualizó a \"{Entidad}\".";
        }
    }

    public sealed class EntidadEliminada<T> : NotificacionEntidad<T> where T : IEntidad
    {
        public EntidadEliminada(T entidad) : base(entidad)
        {
            Titulo = $"Eliminando {typeof(T).Name.ToLower()}";
            Descripcion = $"Se eliminó \"{Entidad}\"";
        }
    }
}
