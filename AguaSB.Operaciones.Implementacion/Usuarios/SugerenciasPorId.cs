using System;
using System.Linq;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Usuarios.Implementacion
{
    public sealed class SugerenciasPorId : IProveedorSugerenciasUsuarios, IAmbitoDependiente
    {
        private ILocalizadorUsuarios Usuarios { get; }

        public SugerenciasPorId(ILocalizadorUsuarios usuarios)
        {
            Usuarios = usuarios ?? throw new ArgumentNullException(nameof(usuarios));
        }

        public IQueryable<Usuario> Obtener(string criterio)
        {
            if (int.TryParse(criterio.Trim(), out int id))
            {
                if (Usuarios.PorId(id) is Usuario usuario)
                    return new[] { usuario }.AsQueryable();
            }

            return Enumerable.Empty<Usuario>().AsQueryable();
        }
    }
}
