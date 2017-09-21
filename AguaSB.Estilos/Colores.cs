using System;
using System.Collections.Generic;
using System.Linq;
using WPF = System.Windows.Media;

using MoreLinq;

namespace AguaSB.Estilos
{
    public static class Colores
    {
        #region Calcular
        public static WPF.Color ColorDe(string colorCadena)
        {
            if (colorCadena == null)
                throw new ArgumentNullException(nameof(colorCadena));

            colorCadena = colorCadena.ToLower();
            return WPF.Color.FromArgb(Int(colorCadena.Take(2)), Int(colorCadena.Slice(2, 2)), Int(colorCadena.Slice(4, 2)), Int(colorCadena.TakeLast(2)));
        }

        private static IEnumerable<KeyValuePair<char, byte>> Ajustar(int cantidad, char inicio, int inicioN) =>
            Enumerable.Range(0, cantidad)
            .Select(i => new KeyValuePair<char, byte>((char)(inicio + i), (byte)(i + inicioN)));

        private static readonly Dictionary<char, byte> Byte = Ajustar(10, '0', 0).Concat(Ajustar(6, 'a', 10)).ToDictionary(kv => kv.Key, kv => kv.Value);

        private static byte Int(IEnumerable<char> enumerable) => enumerable.Fold((c1, c2) => (byte)((Byte[c1] << 4) + Byte[c2]));
        #endregion

        public static Color CalcularColor(string nombre, string codigoHex, string nombreMahApps) =>
            new Color(nombre, nombreMahApps, ColorDe(codigoHex));

        public static readonly Color Azul = CalcularColor(nameof(Azul), "CC119EDA", "Blue");
        public static readonly Color Naranja = CalcularColor(nameof(Naranja), "CCFA6800", "Orange");
        public static readonly Color Verde = CalcularColor(nameof(Verde), "CC60A917", "Green");
        public static readonly Color Acero = CalcularColor(nameof(Acero), "CC647687", "Steel");
        public static readonly Color Cobalto = CalcularColor(nameof(Cobalto), "CC0050EF", "Cobalt");
        public static readonly Color Purpura = CalcularColor(nameof(Purpura), "CC6459DF", "Purple");
        public static readonly Color Cafe = CalcularColor(nameof(Cafe), "CC825A2C", "Brown");
        public static readonly Color Rojo = CalcularColor(nameof(Rojo), "CCE51400", "Red");
        public static readonly Color Rosa = CalcularColor(nameof(Rosa), "CCF472D0", "Pink");
        public static readonly Color Crimson = CalcularColor(nameof(Crimson), "CCA20025", "Crimson");
    }
}
