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

        public static DateTime Hoy => DiaDe(Ahora);

        public static DateTime EsteMes => MesDe(Ahora);

        public static void EstablecerAhora(DateTime ahora) => Ahora = ahora;

        public static DateTime DiaDe(DateTime fecha) => new DateTime(fecha.Year, fecha.Month, fecha.Day);

        public static DateTime MesDe(DateTime fecha) => new DateTime(fecha.Year, fecha.Month, 01);
    }
}
