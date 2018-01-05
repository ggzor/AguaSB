using System;
using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Usuarios.Utilerias
{
    /// <summary>
    /// Advertencia: Para verificar que una secuencia tiene valores primero llamará <see cref="Queryable.Any{TSource}(IQueryable{TSource})"/>
    /// y luego devolverá la secuencia.
    /// </summary>
    public class ProveedorSugerenciasRelevable : IProveedorSugerenciasUsuarios
    {
        public IReadOnlyList<IProveedorSugerenciasUsuarios> Proveedores { get; }

        public ProveedorSugerenciasRelevable(IEnumerable<IProveedorSugerenciasUsuarios> proveedores)
        {
            Proveedores = proveedores?.ToList() ?? throw new ArgumentNullException(nameof(proveedores));
        }

        public IQueryable<Usuario> Obtener(string criterio)
        {
            IQueryable<Usuario> ObtenerValoresDeSegundoSiPrimeroEsVacio(IQueryable<Usuario> primero, IProveedorSugerenciasUsuarios segundo)
            {
                if (primero.Any())
                    return primero;
                else
                    return segundo.Obtener(criterio);
            }

            return Proveedores.Aggregate(Enumerable.Empty<Usuario>().AsQueryable(), ObtenerValoresDeSegundoSiPrimeroEsVacio);
        }
    }
}
