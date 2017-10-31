using AguaSB.Datos;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Nucleo.Datos
{
    public static class Domicilios
    {
        public static IDictionary<Seccion, IList<Calle>> CallesAgrupadas(IRepositorio<Seccion> seccionesRepo) =>
            (from seccion in seccionesRepo.Datos
             orderby seccion.Orden, seccion.Nombre
             select new { Seccion = seccion, Calles = seccion.Calles.OrderBy(calle => calle.Nombre) })
            .ToDictionary(g => g.Seccion, g => (IList<Calle>)g.Calles.ToList());
    }
}
