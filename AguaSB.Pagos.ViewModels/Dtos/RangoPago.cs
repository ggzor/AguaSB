using AguaSB.Nucleo.Pagos;
using AguaSB.ViewModels;
using System;
using System.Collections.Generic;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class RangoPago
    {
        public PagoContrato Padre { get; set; }
        public DateTime Hasta { get; set; }
        public decimal Monto { get; set; }
        public decimal AdeudoRestante { get; set; }

        public bool EsPrimeroConRestanteCero { get; set; }
        public bool NoEsPrimeroConRestanteCero => !EsPrimeroConRestanteCero;

        public IEnumerable<IDetallePago> Detalles { get; set; }

        private bool activo;

        public bool Activo
        {
            get { return Padre.RangoPagoSeleccionado == this; }
            set { if (value) Padre.RangoPagoSeleccionado = this; }
        }

    }
}
