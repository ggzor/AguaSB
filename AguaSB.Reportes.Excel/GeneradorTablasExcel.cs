using System;
using System.IO;

namespace AguaSB.Reportes.Excel
{
    public class GeneradorTablasExcel : IGeneradorTablas
    {
        public string DireccionBase { get; }

        public GeneradorTablasExcel(string direccionTablas)
        {
            DireccionBase = direccionTablas ?? throw new ArgumentNullException(nameof(direccionTablas));
        }

        public ILibroTablas CrearLibro(string nombre)
        {
            var direccion = Path.Combine(DireccionBase, nombre + ".xlsx");
            var archivo = new FileInfo(direccion);

            if (archivo.Exists)
                throw new InvalidOperationException($"Ya existe un libro con el nombre {nombre}");

            return new LibroExcel(nombre, archivo);
        }
    }
}
