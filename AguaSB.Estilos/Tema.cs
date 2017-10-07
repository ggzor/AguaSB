using System;
using System.Windows.Media;

namespace AguaSB.Estilos
{
    public class Tema
    {
        public string Nombre { get; }

        public string NombreMahApps { get; }

        public Color ColorWPF { get; }

        public Brush BrochaWPF { get; }

        public Brush BrochaSolidaWPF { get; }

        public Tema(string nombre, string nombreMahApps, Color colorWPF)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentNullException(nameof(nombre));

            if (string.IsNullOrWhiteSpace(nombreMahApps))
                throw new ArgumentNullException(nameof(nombreMahApps));

            Nombre = nombre;
            NombreMahApps = nombreMahApps;
            ColorWPF = colorWPF;
            BrochaWPF = new SolidColorBrush(colorWPF);

            var colorWPFSolido = Color.FromRgb(colorWPF.R, colorWPF.G, colorWPF.B);
            BrochaSolidaWPF = new SolidColorBrush(colorWPFSolido);

            BrochaWPF.Freeze();
        }
    }
}
