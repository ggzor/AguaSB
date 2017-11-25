using AguaSB.Nucleo;
using Mehdime.Entity;
using System.Linq;
using System.Data.Entity;

namespace AguaSB.Datos.Entity
{
    public class RepositorioContactos : RepositorioEntity<Contacto>
    {
        public RepositorioContactos(IAmbientDbContextLocator localizador) : base(localizador)
        {
        }

        public override Contacto Agregar(Contacto entidad)
        {
            BaseDatos.Entry(entidad.TipoContacto).State = EntityState.Unchanged;

            return base.Agregar(entidad);
        }

        public override Contacto Eliminar(Contacto entidad)
        {
            if (BaseDatos.Contactos.FirstOrDefault(_ => _.Id == entidad.Id) is Contacto c)
                BaseDatos.Contactos.Remove(c);

            return entidad;
        }
    }
}
