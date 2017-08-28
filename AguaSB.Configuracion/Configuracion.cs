using System;
using System.IO;

using Newtonsoft.Json;

namespace AguaSB.Configuracion
{
    /// <summary>
    /// Utilerías para cargar configuraciones con facilidad. Es básicamente un adaptador para JSON.
    /// </summary>
    public static class Configuracion
    {

        public const string SubdirectorioDeConfiguracion = "Configuracion";

        public const string ExtensionDeArchivos = ".json";

        public static T Cargar<T>(FileInfo direccion)
        {
            string ExtraerTexto()
            {
                using (var stream = direccion.OpenText())
                    return stream.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(ExtraerTexto());
        }

        /// <summary>
        /// Carga el archivo "<paramref name="subdirectorio"/>/<typeparamref name="T"/>.<paramref name="extension"/>"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subdirectorio"></param>
        /// <returns></returns>
        public static T Cargar<T>(string subdirectorio = SubdirectorioDeConfiguracion, string extension = ".json") =>
            Cargar<T>(new FileInfo(Combinar(subdirectorio, typeof(T), extension)));


        public static string Combinar(string subdirectorio, Type tipo, string extension)
        {
            string Simplificar(string s) => s.Trim('/', '\\');

            return Simplificar(Simplificar(subdirectorio) + "/" + tipo.Name + extension);
        }


        public static void Guardar(object objeto, FileInfo direccion)
        {
            var texto = JsonConvert.SerializeObject(objeto);

            using (var stream = new StreamWriter(direccion.Open(FileMode.Create)))
                stream.Write(texto);
        }

        public static void Guardar(object objeto, string subdirectorio = SubdirectorioDeConfiguracion, string extension = ExtensionDeArchivos) =>
            Guardar(objeto, new FileInfo(Combinar(subdirectorio, objeto.GetType(), extension)));
    }
}
