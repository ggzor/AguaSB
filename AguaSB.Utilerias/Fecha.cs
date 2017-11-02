using System;

namespace AguaSB.Utilerias
{
    public static class Fecha
    {
        private static DateTime? ahora;

        public static DateTime Ahora
        {
            get { return ahora ?? DateTime.Now; }
            set { ahora = value; }
        }

        public static void EstablecerAhora(DateTime ahora) => Ahora = ahora;
    }
}
