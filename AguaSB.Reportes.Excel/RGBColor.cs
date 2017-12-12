using System.Drawing;

namespace AguaSB.Reportes.Excel
{
    public static class RGBColor
    {
        public static Color AColor(this RGB color) => Color.FromArgb(color.R, color.G, color.B);
    }
}
