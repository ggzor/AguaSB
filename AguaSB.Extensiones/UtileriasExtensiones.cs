using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Monad;

namespace AguaSB.Extensiones
{
    public static class UtileriasExtensiones
    {
        public static IEnumerable<(string Archivo, TryResult<IExtension> Extension)> En(string directorio, Func<string, bool> filtro)
        {
            var archivosAVerificar = Directory.EnumerateFiles(directorio).Where(filtro);

            return from archivo in archivosAVerificar
                   let intento = IntentarCargarExtension(archivo)
                   select (archivo, intento.Try());
        }

        private static Try<IExtension> IntentarCargarExtension(string archivo) =>
            () => new TryResult<IExtension>(CargarExtension(archivo));

        private static IExtension CargarExtension(string archivo)
        {
            var ensamblado = Assembly.LoadFile(archivo);

            var tipoExtension = (from tipo in ensamblado.GetTypes()
                                 where tipo.IsClass
                                 where typeof(IExtension).IsAssignableFrom(tipo)
                                 select tipo).Single();

            return (IExtension)Activator.CreateInstance(tipoExtension);
        }
    }
}
