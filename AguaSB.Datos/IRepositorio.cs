using System.Threading.Tasks;

using AguaSB.Nucleo;

namespace AguaSB.Datos
{
    public interface IRepositorio<T> where T : IEntidad
    {
        Task<T> Agregar(T entidad);
    }
}
