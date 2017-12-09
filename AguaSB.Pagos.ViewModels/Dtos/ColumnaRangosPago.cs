using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class ColumnaRangosPago
    {
        public EncabezadoRangosPago Encabezado { get; set; }

        public RangoPago[] RangosPago { get; set; }

        public ColumnaRangosPago(int conteoInicio, int conteoFin, IEnumerable<RangoPago> rangosPago)
        {
            if (rangosPago == null)
                throw new ArgumentNullException(nameof(rangosPago));

            if (!rangosPago.Any())
                throw new ArgumentException("Debe haber al menos un rango en la colección.", nameof(rangosPago));

            RangosPago = rangosPago.ToArray();

            var primero = RangosPago[0];
            var ultimo = RangosPago.Last();

            Encabezado = new EncabezadoRangosPago
            {
                ConteoInicio = conteoInicio,
                ConteoFin = conteoFin,
                MesInicio = primero.Hasta,
                MesFin = ultimo.Hasta,
                MontoInicio = primero.Monto,
                MontoFin = ultimo.Monto,
            };
        }
    }
}