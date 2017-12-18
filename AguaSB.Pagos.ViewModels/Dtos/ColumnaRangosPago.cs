using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class ColumnaRangosPago
    {
        public int ConteoInicio { get; }
        public int ConteoFin { get; }

        public RangoPago Inicio { get; }
        public RangoPago Fin { get; }

        public IReadOnlyCollection<RangoPago> RangosPago { get; }

        public ColumnaRangosPago(int conteoInicio, int conteoFin, IReadOnlyCollection<RangoPago> rangosPago)
        {
            RangosPago = rangosPago ?? throw new ArgumentNullException(nameof(rangosPago));

            if (!rangosPago.Any())
                throw new ArgumentException("Debe haber al menos un rango en la colección.", nameof(rangosPago));

            ConteoInicio = conteoInicio;
            ConteoFin = conteoFin;
            Inicio = rangosPago.First();
            Fin = rangosPago.Last();
        }
    }
}