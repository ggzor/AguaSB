using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AguaSB.Extensiones
{
    /// <summary>
    /// Carga archivos *.dll especificados que contengan la clase especificada.
    /// </summary>
    public class CargadorDeExtensiones
    {

        public IEnumerable<T> Cargar<T>(FileInfo archivo) where T : class
        {
            if (archivo == null)
                throw new ArgumentNullException(nameof(archivo));

            var ensamblado = Assembly.LoadFile(archivo.FullName);

            return from tipo in ensamblado.GetTypes()
                   where tipo.IsSubclassOf(typeof(T))
                   select Activator.CreateInstance(tipo) as T;
        }
    }
}
