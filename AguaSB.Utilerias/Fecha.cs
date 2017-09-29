using System;

namespace AguaSB.Utilerias
{
    public sealed class Fecha
    {
        public DateTime DateTime { get; }

        public Fecha(DateTime date)
        {
            DateTime = date;
        }

        private static Fecha ahora;

        public static Fecha Ahora
        {
            get { return ahora ?? new Fecha(DateTime.Now); }
            set { ahora = value; }
        }

        public static void EstablecerAhora(DateTime ahora) => Fecha.ahora = new Fecha(ahora);
    }
}
