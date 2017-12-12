using System;

namespace AguaSB.Reportes
{
    public class RGB
    {
        public int R { get; }

        public int G { get; }

        public int B { get; }

        public RGB(int r, int g, int b)
        {
            Validar(r, nameof(r));
            Validar(g, nameof(g));
            Validar(b, nameof(b));

            R = r;
            G = g;
            B = b;
        }

        private void Validar(int c, string name)
        {
            if (c < 0 || 255 < c)
                throw new ArgumentOutOfRangeException(name);
        }
    }
}
