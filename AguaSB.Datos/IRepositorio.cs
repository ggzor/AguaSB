using System.Threading.Tasks;

namespace AguaSB.Datos
{
    public interface IRepositorio<T>
    {
        Task<T> Agregar(T entidad);
    }
}
