using System.Collections.Generic;
using System.Threading.Tasks;

using AguaSB.Nucleo;

namespace AguaSB.Datos
{
    public class RepositorioEnMemoria<T> : IRepositorio<T> where T : IEntidad
    {
        private readonly List<T> Entidades = new List<T>();

        public IEnumerable<T> Datos => Entidades;

        public Task<T> Agregar(T entidad)
        {
            Entidades.Add(entidad);

            entidad.Id = Entidades.Count;

            return Task.FromResult(entidad);
        }
    }
}