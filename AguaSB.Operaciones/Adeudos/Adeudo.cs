using System.Collections.Generic;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Montos;

namespace AguaSB.Operaciones.Adeudos
{
    public struct Adeudo
    {
        public Contrato Contrato { get; set; }
        public Pago UltimoPago { get; set; }
        public decimal Cantidad { get; set; }
        public IEnumerable<IDetalleMonto> Detalles { get; set; }
    }
}