using OfficeOpenXml;
using System;

namespace AguaSB.Servicios.Office
{
    public sealed class EditorFilaExcel
    {
        public ExcelWorksheet Hoja { get; }

        public int Fila { get; }

        public EditorFilaExcel(ExcelWorksheet hoja, int fila)
        {
            if (fila <= 0)
                throw new ArgumentOutOfRangeException(nameof(fila));

            Hoja = hoja ?? throw new ArgumentNullException(nameof(hoja));
            Fila = fila;
        }

        public void Establecer<T>(int columna, T valor) => Hoja.Cells[Fila, columna].Value = valor;
        public T Obtener<T>(int columna) => Hoja.Cells[Fila, columna].GetValue<T>();

        public void Establecer(int columna, DateTime fecha)
        {
            Hoja.Cells[Fila, columna].Style.Numberformat.Format = "dd/mm/yyyy";
            Establecer<DateTime>(columna, fecha);
        }
    }
}
