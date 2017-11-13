using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Utilerias.Solicitudes
{
    public sealed class Solicitud
    {
        public List<Propiedad> Agrupadores { get; set; } = new List<Propiedad>();

        public List<Propiedad> Columnas { get; set; } = new List<Propiedad>();

        public List<Condicion> Filtros { get; set; } = new List<Condicion>();

        public List<Propiedad> Ordenamientos { get; set; } = new List<Propiedad>();

        public bool IntentarObtenerFiltro<T>(string propiedad, out T filtro) where T : Condicion
        {
            var posiblesFiltros = from f in Filtros
                                  where f.GetType() == typeof(T)
                                  where f.Propiedad.Nombre == propiedad
                                  select (T)f;

            filtro = posiblesFiltros.FirstOrDefault();
            return filtro != null;
        }
    }
}
