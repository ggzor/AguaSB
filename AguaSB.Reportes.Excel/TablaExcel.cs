using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AguaSB.Reportes.Excel
{
    public sealed class TablaExcel : ITabla, IColor, IFormato
    {
        public string Nombre { get; }
        private ExcelWorksheet Hoja { get; }

        public TablaExcel(string nombre, ExcelWorksheet hoja)
        {
            Nombre = nombre;
            Hoja = hoja;

            Escritor = new EscritorTablaExcel(hoja);
        }

        public IFormato Formato => this;

        public IColor Color => this;

        public IEscritorTabla Escritor { get; }

        string IFormato.this[int x, int y]
        {
            get => Hoja.Cells[y, x].Style.Numberformat.Format;
            set => Hoja.Cells[y, x].Style.Numberformat.Format = value;
        }

        public object this[int x, int y]
        {
            get => Hoja.Cells[y, x].Value;
            set => Hoja.Cells[y, x].Value = value;
        }

        public object this[int xi, int yi, int xf, int yf]
        {
            get => Hoja.Cells[yi, xi, yf, xf].Value;
            set
            {
                var celdas = Hoja.Cells[yi, xi, yf, xf];
                celdas.Merge = true;
                celdas.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                celdas.Value = value;
            }
        }

        RGB IColor.this[int x, int y] { get => Color[x, y]; set => Color[x, y] = value; }
        RGB IColor.this[int xi, int yi, int xf, int yf] { get => Color[xi, yi, xf, yf]; set => Color[xi, yi, xf, yf] = value; }
    }
}
