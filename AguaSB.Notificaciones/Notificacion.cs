using System;

namespace AguaSB.Notificaciones
{
    public abstract class Notificacion
    {
        public DateTime Fecha { get; } = Utilerias.Fecha.Ahora.DateTime;
    }
}