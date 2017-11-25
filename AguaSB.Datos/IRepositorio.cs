using System.Linq;
using AguaSB.Nucleo;

namespace AguaSB.Datos
{
    public interface IRepositorio<T> where T : class, IEntidad
    {
        IQueryable<T> Datos { get; }

        T Agregar(T entidad);
        T Actualizar(T entidad);
        T Eliminar(T entidad);
    }
}
