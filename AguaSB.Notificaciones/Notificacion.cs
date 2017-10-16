using System;

namespace AguaSB.Notificaciones
{
    public abstract class Notificacion
    {
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string Clase { get; set; }

        public DateTime Fecha { get; set; } = Utilerias.Fecha.Ahora.DateTime;
    }
}