using System;
using System.Linq;

using AguaSB.Nucleo;
using MoreLinq;

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

            if (palabras.AtMost(5))
                return ExtraerDeMultiplesPalabras(palabras);
            else
                return Usuarios.PorNombre(texto);
        }

        private IQueryable<Usuario> ExtraerDeMultiplesPalabras(string[] palabras)
        {
            var resultado = Usuarios.Todos.Select(u => new { Usuario = u, Prioridad = 0f });

            for (int i = 0; i < palabras.Length; i++)
            {
                var multiplicador = 0f;

                switch (i)
                {
                    case 0:
                        multiplicador = 1;
                        break;
                    case 1:
                        multiplicador = 1;
                        break;
                    case 2:
                        multiplicador = 1;
                        break;
                    default:
                        multiplicador = Math.Max(1 / (i - 1), 0.4f);
                        break;
                }

                var palabra = palabras[i];
                var palabraCompleta = $" {palabra} ";
                var palabraInicio = palabraCompleta.TrimStart(' ');
                var palabraFinal = palabraCompleta.TrimEnd(' ');

                resultado = from par in resultado
                            let usuario = par.Usuario
                            let nombre = usuario.NombreSolicitud
                            let prioridad = par.Prioridad
                            let nuevaPrioridad =
                                (nombre.Contains(palabraCompleta) ? 7 : 0) +
                                (nombre.StartsWith(palabraInicio) ? 9 : 0) +
                                (nombre.EndsWith(palabraFinal) ? 5 : 0) +
                                (nombre.Contains(palabra) ? 3 : 0)
                            where nombre.Contains(palabra)
                            select new { Usuario = usuario, Prioridad = prioridad + (nuevaPrioridad * multiplicador) };
            }

            return from par in resultado
                   where par.Prioridad != 0
                   orderby par.Prioridad descending, par.Usuario.NombreSolicitud ascending
                   select par.Usuario;
        }
    }
}
