using AguaSB.Nucleo;
using Mehdime.Entity;

using System;
using System.Data.Entity;
using System.Linq;

namespace AguaSB.Datos.Entity
{
    public class RepositorioEntity<T> : IRepositorio<T> where T : class, IEntidad
    {
        public IAmbientDbContextLocator Localizador { get; }

        public EntidadesDbContext BaseDatos
        {
            get
            {
                return Localizador.Get<EntidadesDbContext>()
                    ?? throw new InvalidOperationException("No se pudo obtener la base de datos en el contexto actual.");
            }
        }

        public RepositorioEntity(IAmbientDbContextLocator localizador) =>
            Localizador = localizador ?? throw new ArgumentNullException(nameof(localizador));

        public IQueryable<T> Datos => BaseDatos.Set<T>();

        public virtual T Actualizar(T entidad)
        {
            BaseDatos.Set<T>().Attach(entidad);

            BaseDatos.Entry(entidad).State = EntityState.Modified;

            return entidad;
        }

        public virtual T Agregar(T entidad)
        {
            BaseDatos.Set<T>().Add(entidad);

            return entidad;
        }

        public virtual T Eliminar(T entidad)
        {
            BaseDatos.Set<T>().Attach(entidad);
            BaseDatos.Entry(entidad).State = EntityState.Deleted;

            return entidad;
        }
    }
}
