using System;

namespace AguaSB.Configuracion
{
    public static class UtileriasDirectorios
    {

        public static string Combinar(string subdirectorio, string nombre, string extension)
        {
            if (nombre == null)
                throw new ArgumentNullException(nameof(nombre));


            string Simplificar(string s) => s.Trim('/', '\\');

            return Simplificar(Simplificar(subdirectorio ?? string.Empty) + "/" + nombre + (extension ?? string.Empty));
        }
    }
}
