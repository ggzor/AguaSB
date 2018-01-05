using System;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Notas;

namespace AguaSB.Operaciones.Usuarios.Legado
{
    public class SugerenciasPorNota : IProveedorSugerenciasUsuarios
    {
        public ILocalizadorUsuarios LocalizadorUsuarios { get; }
        public ILocalizadorNotas Notas { get; }

        public SugerenciasPorNota(ILocalizadorUsuarios localizadorUsuarios, ILocalizadorNotas localizadorNotas)
        {
            LocalizadorUsuarios = localizadorUsuarios ?? throw new ArgumentNullException(nameof(localizadorUsuarios));
            Notas = localizadorNotas ?? throw new ArgumentNullException(nameof(localizadorNotas));
        }

        public IQueryable<Usuario> Obtener(string criterio)
        {
            if (int.TryParse(criterio, out var id))
            {
                var idEnCadena = id.ToString();
                var notaFila = (from nota in Notas.DelTipo("_Usuario_NumeroUsuario")
                                where nota.Informacion == idEnCadena
                                select nota)
                               .SingleOrDefault();

                if (notaFila?.Referencia is int idUsuario)
                    return new[] { LocalizadorUsuarios.PorId(idUsuario) }.AsQueryable();
            }

            return Enumerable.Empty<Usuario>().AsQueryable();
        }
    }
}
