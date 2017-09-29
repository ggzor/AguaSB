using System.Collections.Generic;
using System.Threading.Tasks;

using AguaSB.Nucleo;

namespace AguaSB.Datos
{
    public interface IRepositorio<T> where T : IEntidad
    {
        IEnumerable<T> Datos { get; }

        Task<T> Agregar(T entidad);
    }
}
