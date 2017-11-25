using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;

namespace AguaSB.Datos
{
    public class RepositorioEnMemoria<T> : IRepositorio<T> where T : class, IEntidad
    {
        private readonly List<T> Entidades = new List<T>();

        public IQueryable<T> Datos => Entidades.AsQueryable();

        public T Agregar(T entidad)
        {
            Entidades.Add(entidad);

            entidad.Id = Entidades.Count;

            return entidad;
        }

        public T Actualizar(T entidad) => entidad;

        public T Eliminar(T entidad)
        {
            Entidades.Remove(entidad);
            return entidad;
        }
    }
}