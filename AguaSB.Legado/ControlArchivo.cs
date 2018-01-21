using System;
using OfficeOpenXml;

namespace AguaSB.Legado
{
    internal sealed class ControlArchivo
    {
        public ExcelPackage Archivo { get; }

        public ControlArchivo(ExcelPackage archivo) => Archivo = archivo ?? throw new ArgumentNullException(nameof(archivo));

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
