using OfficeOpenXml;
using System.Linq;

namespace AguaSB.Legado
{
    public static class LecturaFolioPago
    {
        /// <summary>
        /// Encuentra la primera fila con datos (basado en columna) yendo desde 
        /// inicio en reversa, y regresa el indice de la siguiente fila.
        /// </summary>
        public static (int Fila, int Folio) Obtener(ExcelWorksheet hoja, int inicioBusqueda, int columnaBusqueda, int columnaFolio)
        {
            return (from i in Enumerable.Range(0, inicioBusqueda - 1)
                    let fila = inicioBusqueda - i
                    let valor = hoja.Cells[fila, columnaBusqueda].GetValue<int?>()
                    where valor != null
                    select (fila + 1, hoja.Cells[fila + 1, columnaFolio].GetValue<int>()))
           .First();
        }
    }
}
