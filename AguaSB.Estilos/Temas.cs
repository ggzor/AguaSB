using System.Windows.Media;

namespace AguaSB.Estilos
{
    public static class Temas
    {
        public static Color ColorDe(string colorCadena) => (Color)ColorConverter.ConvertFromString(colorCadena);

        public static readonly Tema Azul = new Tema(nameof(Azul), "Blue", ColorDe("#CC119EDA"));
        public static readonly Tema Naranja = new Tema(nameof(Naranja), "Orange", ColorDe("#CCFA6800"));
        public static readonly Tema Verde = new Tema(nameof(Verde), "Green", ColorDe("#CC60A917"));
        public static readonly Tema Acero = new Tema(nameof(Acero), "Steel", ColorDe("#CC647687"));
        public static readonly Tema Cobalto = new Tema(nameof(Cobalto), "Cobalt", ColorDe("#CC0050EF"));
        public static readonly Tema Purpura = new Tema(nameof(Purpura), "Purple", ColorDe("#CC6459DF"));
        public static readonly Tema Cafe = new Tema(nameof(Cafe), "Brown", ColorDe("#CC825A2C"));
        public static readonly Tema Rojo = new Tema(nameof(Rojo), "Red", ColorDe("#CCE51400"));
        public static readonly Tema Rosa = new Tema(nameof(Rosa), "Pink", ColorDe("#CCF472D0"));
        public static readonly Tema Crimson = new Tema(nameof(Crimson), "Crimson", ColorDe("#CCA20025"));
    }
}
