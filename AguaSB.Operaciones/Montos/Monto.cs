using System.Collections.Generic;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Montos
{
    public struct Monto
    {
        public Contrato Contrato { get; set; }
        public decimal Cantidad { get; set; }
        public IEnumerable<IDetalleMonto> Detalles { get; set; }
    }
}
