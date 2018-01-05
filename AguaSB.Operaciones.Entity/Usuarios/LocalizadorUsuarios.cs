using System.Linq;

using Mehdime.Entity;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Entity;

namespace AguaSB.Operaciones.Usuarios.Entity
{
    public class LocalizadorUsuarios : OperacionesEntity, ILocalizadorUsuarios
    {
        public LocalizadorUsuarios(IAmbientDbContextLocator localizador) : base(localizador)
        {
        }

        public IQueryable<Usuario> Todos => BaseDeDatos.Usuarios;

        public Usuario PorId(int id) => BaseDeDatos.Usuarios.Find(id);

        public IQueryable<Usuario> PorNombre(string nombre)
        {
            var nombreProcesado = Usuario.ConvertirATextoSolicitud(nombre);

            return from u in BaseDeDatos.Usuarios
                   where u.NombreSolicitud.Contains(nombreProcesado)
                   select u;
        }
    }
}
