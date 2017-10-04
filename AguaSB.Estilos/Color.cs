using System;
using WPF = System.Windows.Media;

namespace AguaSB.Estilos
{
    public class Color
    {
        public string Nombre { get; }

        public string NombreMahApps { get; }

        public WPF.Color ColorWPF { get; }

        public WPF.Brush BrochaWPF { get; }

        public WPF.Brush BrochaSolidaWPF { get; }

        public Color(string nombre, string nombreMahApps, WPF.Color colorWPF)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentNullException(nameof(nombre));

            if (string.IsNullOrWhiteSpace(nombreMahApps))
                throw new ArgumentNullException(nameof(nombreMahApps));

            Nombre = nombre;
            NombreMahApps = nombreMahApps;
            ColorWPF = colorWPF;
            BrochaWPF = new WPF.SolidColorBrush(colorWPF);

            var colorWPFSolido = WPF.Color.FromRgb(colorWPF.R, colorWPF.G, colorWPF.B);
            BrochaSolidaWPF = new WPF.SolidColorBrush(colorWPFSolido);

            BrochaWPF.Freeze();
        }
    }
}
