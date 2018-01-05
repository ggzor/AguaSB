using System;
using System.Linq;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Usuarios.Implementacion
{
    /// TODO: Mejorar algoritmo para ordenar coincidencias
    public class SugerenciasPorNombre : IProveedorSugerenciasUsuarios
    {
        public ILocalizadorUsuarios Usuarios { get; }

        public SugerenciasPorNombre(ILocalizadorUsuarios usuarios)
        {
            Usuarios = usuarios ?? throw new ArgumentNullException(nameof(usuarios));
        }

        public IQueryable<Usuario> Obtener(string criterio) => Usuarios.PorNombre(criterio);
    }
}
