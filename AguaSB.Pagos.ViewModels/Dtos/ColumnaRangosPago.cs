using System.Collections.Generic;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class ColumnaRangosPago
    {
        public EncabezadoRangosPago Encabezado { get; set; }

        public IEnumerable<RangoPagoSeleccionable> RangosPago { get; set; }
    }
}