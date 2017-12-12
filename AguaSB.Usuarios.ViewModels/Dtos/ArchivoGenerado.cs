using AguaSB.Notificaciones;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class ArchivoGenerado : Notificacion
    {
        public ArchivoGenerado(string nombreArchivo)
        {
            Clase = "NuevoArchivo";
            Fecha = Utilerias.Fecha.Ahora;
            Titulo = "Archivo generado";
            Descripcion = $"Se generó correctamente el archivo \"{nombreArchivo}\"";
        }
    }
}
