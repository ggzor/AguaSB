using System.Collections.Generic;
using System.Threading.Tasks;

using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.Proveedores
{
    public class RepositorioEnMemoria<T> : IRepositorio<T> where T : IEntidad
    {
        private List<T> Entidades = new List<T>();

        public Task<T> Agregar(T entidad)
        {
            Entidades.Add(entidad);

            entidad.Id = Entidades.Count;

            return Task.FromResult(entidad);
        }
    }
}