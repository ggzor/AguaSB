using System;

namespace AguaSB.Utilerias.IO
{
    public static class UtileriasDirectorios
    {
        /// <summary>
        /// Crea la dirección al archivo "<paramref name="subdirectorio"/>/<paramref name="nombre"/>.<paramref name="extension"/>"
        /// limpiando los caracteres laterales en los argumentos y en el resultado.
        /// </summary>
        public static string Combinar(string subdirectorio, string nombre, string extension)
        {
            if (nombre == null)
                throw new ArgumentNullException(nameof(nombre));


            string Simplificar(string s) => s.Trim('/', '\\');

            return Simplificar(Simplificar(subdirectorio ?? string.Empty) + "/" + nombre + (extension ?? string.Empty));
        }
    }
}
