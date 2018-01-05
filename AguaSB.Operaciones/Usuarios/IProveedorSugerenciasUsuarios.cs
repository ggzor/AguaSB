using AguaSB.Nucleo;
using System.Linq;

namespace AguaSB.Operaciones.Usuarios
{
    public interface IProveedorSugerenciasUsuarios
    {
        IQueryable<Usuario> Obtener(string criterio);
    }
}
