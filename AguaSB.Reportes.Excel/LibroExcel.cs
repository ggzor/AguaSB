using OfficeOpenXml;
using System;
using System.IO;

namespace AguaSB.Reportes.Excel
{
    public class LibroExcel : ILibroTablas
    {
        public string Nombre { get; }

        private ExcelPackage Documento { get; }

        public LibroExcel(string nombre, FileInfo archivo)
        {
            if (archivo == null)
                throw new ArgumentNullException(nameof(archivo));

            Documento = new ExcelPackage(archivo);
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        }

        public ITabla CrearNueva(string nombre)
        {
            var hoja = Documento.Workbook.Worksheets.Add(nombre);

            return new TablaExcel(nombre, hoja);
        }

        public void Generar()
        {
            Documento.Save();
            Documento.Dispose();
        }
    }
}
