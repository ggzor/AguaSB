using System.Collections;
using System.Collections.Generic;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class PuntoNavegacion
    {
        public string Nombre { get; set; }

        public int Indice { get; set; }

        public override string ToString() => Nombre;
    }

    public class ResultadoSolicitud
    {
        public IEnumerable Resultados { get; set; }

        public long Conteo { get; set; }

        public IEnumerable<PuntoNavegacion> PuntosNavegacion { get; set; }
    }
}
