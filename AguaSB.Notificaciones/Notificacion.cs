using System;

namespace AguaSB.Notificaciones
{
    public abstract class Notificacion
    {
        public abstract string Titulo { get; }

        public abstract string Descripcion { get; }

        public abstract string Clase { get; }

        public DateTime Fecha { get; } = Utilerias.Fecha.Ahora.DateTime;
    }
}