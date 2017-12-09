using System;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class EncabezadoRangosPago
    {
        public decimal MontoInicio { get; set; }
        public decimal MontoFin { get; set; }

        public DateTime MesInicio { get; set; }
        public DateTime MesFin { get; set; }

        public int ConteoInicio { get; set; }
        public int ConteoFin { get; set; }
    }
}