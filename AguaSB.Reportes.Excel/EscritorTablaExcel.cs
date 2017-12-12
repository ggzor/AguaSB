using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AguaSB.Reportes.Excel
{
    internal class EscritorTablaExcel : IEscritorTabla
    {
        private ExcelWorksheet Hoja { get; }

        public EscritorTablaExcel(ExcelWorksheet hoja)
        {
            Hoja = hoja;
        }

        public RGB ColorEncabezado { get; set; }
        public RGB ColorTextoEncabezado { get; set; }

        public void EscribirEncabezado(IEnumerable<string> encabezado)
        {
            const int fila = 1;
            var columna = 1;
            foreach (var cadena in encabezado)
            {
                var celda = Hoja.Cells[fila, columna];

                celda.Value = cadena;
                celda.Style.Font.Bold = true;
                celda.Style.Font.Size += 2;

                var relleno = celda.Style.Fill;
                    relleno.PatternType = ExcelFillStyle.Solid;

                if (ColorEncabezado != null)
                    relleno.BackgroundColor.SetColor(ColorEncabezado.AColor());

                if (ColorTextoEncabezado != null)
                    celda.Style.Font.Color.SetColor(ColorTextoEncabezado.AColor());

                columna++;
            }
        }
    }
}