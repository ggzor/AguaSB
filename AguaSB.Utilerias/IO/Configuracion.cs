using System;
using System.IO;

using Newtonsoft.Json;

using static AguaSB.Utilerias.IO.UtileriasDirectorios;

namespace AguaSB.Utilerias.IO
{
    /// <summary>
    /// Utilerías para cargar configuraciones con facilidad. Es básicamente un adaptador para JSON.
    /// </summary>
    public static class Configuracion
    {
        public const string ExtensionDeArchivos = ".json";

        public static T Cargar<T>(FileInfo direccion)
        {
            if (direccion == null)
                throw new ArgumentNullException(nameof(direccion));

            string ExtraerTexto()
            {
                using (var stream = direccion.OpenText())
                    return stream.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(ExtraerTexto());
        }

        public static T Cargar<T>(string subdirectorio = null, string extension = ExtensionDeArchivos) =>
            Cargar<T>(new FileInfo(Combinar(subdirectorio, typeof(T).Name, extension)));

        public static T Cargar<T>(string nombre, string subdirectorio = null, string extension = ExtensionDeArchivos) =>
            Cargar<T>(new FileInfo(Combinar(subdirectorio, nombre, extension)));


        public static void Guardar(object objeto, FileInfo direccion, bool indentar = true)
        {
            if (objeto == null)
                throw new ArgumentNullException(nameof(objeto));

            if (direccion == null)
                throw new ArgumentNullException(nameof(direccion));


            if (!direccion.Directory.Exists)
                direccion.Directory.Create();

            var formato = indentar ? Formatting.Indented : Formatting.None;
            var texto = JsonConvert.SerializeObject(objeto, formato);

            using (var stream = new StreamWriter(direccion.Open(FileMode.Create)))
                stream.Write(texto);
        }

        public static void Guardar(object objeto, string subdirectorio = null, string extension = ExtensionDeArchivos, bool indentar = true) =>
            Guardar(objeto, new FileInfo(Combinar(subdirectorio, objeto.GetType().Name, extension)), indentar);

        public static void Guardar(object objeto, string nombre, string subdirectorio = null, string extension = ExtensionDeArchivos, bool indentar = true) =>
            Guardar(objeto, new FileInfo(Combinar(subdirectorio, nombre, extension)), indentar);

    }
}
