using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AguaSB.Utilerias.IO
{
    /// <summary>
    /// Carga archivos *.dll especificados que contengan la clase especificada.
    /// </summary>
    public static class CargadorModulos
    {

        public static IEnumerable<T> Cargar<T>(FileInfo archivo) where T : class
        {
            if (archivo == null)
                throw new ArgumentNullException(nameof(archivo));

            var tipoAEncontrar = typeof(T);

            if (tipoAEncontrar.IsSealed)
                return Enumerable.Empty<T>();                          

            var ensamblado = Assembly.LoadFile(archivo.FullName);

            return from tipo in ensamblado.GetTypes()
                   where tipoAEncontrar.IsAssignableFrom(tipo)
                   select Activator.CreateInstance(tipo) as T;
        }
    }
}
