using System;
using System.Linq;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Usuarios.Implementacion
{
    /// TODO: Mejorar algoritmo para ordenar coincidencias
    public class SugerenciasPorNombre : IProveedorSugerenciasUsuarios
    {
        public ILocalizadorUsuarios Usuarios { get; }

        public SugerenciasPorNombre(ILocalizadorUsuarios usuarios)
        {
            Usuarios = usuarios ?? throw new ArgumentNullException(nameof(usuarios));
        }

        public IQueryable<Usuario> Obtener(string criterio)
        {
            var texto = Usuario.ConvertirATextoSolicitud(criterio);

            var unico = Usuarios.Todos.SingleOrDefault(u => u.NombreSolicitud == texto);
            if (unico != null)
                return new[] { unico }.AsQueryable();

            var palabras = texto.Split(' ');

            if (palabras.Count() == 1)
                return ExtraerDePalabraUnica(palabras[0]);
            else
                return Usuarios.PorNombre(texto);
        }

        private IQueryable<Usuario> ExtraerDePalabraUnica(string palabra)
        {
            var palabraCompleta = $" {palabra} ";
            var palabraInicio = palabraCompleta.TrimStart(' ');
            var palabraFinal = palabraCompleta.TrimEnd(' ');

            return from u in Usuarios.Todos
                   let nombre = u.NombreSolicitud
                   let prioridad =
                        (nombre.Contains(palabraCompleta) ? 10 : 0) +
                        (nombre.StartsWith(palabraInicio) ? 7 : 0) +
                        (nombre.EndsWith(palabraFinal) ? 5 : 0)
                   where nombre.Contains(palabra)
                   orderby prioridad descending, nombre ascending
                   select u;
        }
    }
}
