using System.Linq;
using System.Threading.Tasks;

using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.Operaciones
{
    public static class OperacionesUsuarios
    {
        public static Usuario BuscarDuplicados(Usuario usuario, IRepositorio<Usuario> repositorio) =>
            repositorio.Datos.SingleOrDefault(u => u.NombreCompleto == usuario.NombreCompleto);

        public static Task<Usuario> BuscarDuplicadosAsync(Usuario usuario, IRepositorio<Usuario> repositorio) =>
            Task.Run(() => BuscarDuplicados(usuario, repositorio));
    }
}
