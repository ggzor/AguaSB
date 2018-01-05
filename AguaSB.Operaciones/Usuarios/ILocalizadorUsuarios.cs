using System.Linq;
using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Usuarios
{
    public interface ILocalizadorUsuarios
    {
        IQueryable<Usuario> Todos { get; }

        Usuario PorId(int id);
        IQueryable<Usuario> PorNombre(string nombre);
    }
}
