using System;
using System.IO;
using OfficeOpenXml;

namespace AguaSB.Legado
{
    internal sealed class ControlArchivo
    {
        public ExcelPackage Archivo { get; }

        public ControlArchivo(FileInfo archivo)
        {
            if (archivo == null)
                throw new ArgumentNullException(nameof(archivo));

            AsegurarPuedeLeerEscribir(archivo);

            Archivo = new ExcelPackage(archivo);
        }

        private static void AsegurarPuedeLeerEscribir(FileInfo archivo)
        {
            try
            {
                archivo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None).Dispose();
            }
            catch (IOException ex)
            {
                if (archivo.Exists)
                    throw new Exception("El archivo está siendo utilizado. Cierre el archivo antes de realizar el pago.", ex);
                else
                    throw new Exception($"No se puede abrir el archivo {archivo.FullName}", ex);
            }
        }

        private bool cerrado;

        public void Guardar() => SiNoEstaCerrado(() => Archivo.Save());

        public void NoGuardar() => SiNoEstaCerrado(() => Archivo.Dispose());

        private readonly object Lock = new object();

        private void SiNoEstaCerrado(Action accion)
        {
            lock (Lock)
            {
                if (!cerrado)
                {
                    accion();
                    cerrado = true;
                }
            }
        }
    }
}
