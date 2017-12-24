using System;

namespace AguaSB.Legado.Nucleo
{
    public class SolicitudPago
    {
        public int Fila { get; set; }
        public DateTime FechaPago { get; set; }

        public DateTime MesInicio { get; set; }
        public DateTime MesFin { get; set; }
        public int Monto { get; set; }

        public int CantidadMeses => DiferenciaMeses(MesInicio, MesFin);
        public string Meses => GenerarMeses(MesInicio, MesFin);

        private static string GenerarMeses(DateTime inicio, DateTime fin)
        {
            if (inicio == fin)
                return MesAño(inicio).ToUpper();
            else if (inicio.Year == fin.Year)
                return $"{inicio.ToString("MMMM")} - {MesAño(fin)}".ToUpper();
            else
                return $"{MesAño(inicio)} - {MesAño(fin)}".ToUpper();
        }

        private static string MesAño(DateTime fecha) => fecha.ToString("MMMM yyyy");

        private int DiferenciaMeses(DateTime mesInicio, DateTime mesFin) => MesAbsoluto(mesFin) - MesAbsoluto(mesInicio) + 1;
        private int MesAbsoluto(DateTime mes) => (mes.Year * 12) + (mes.Month - 1);
    }
}
