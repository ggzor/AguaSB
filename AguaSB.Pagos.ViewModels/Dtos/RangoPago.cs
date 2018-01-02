using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

using AguaSB.Nucleo.Pagos;
using AguaSB.Utilerias;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class RangoPago : Notificante
    {
        public PagoContrato Padre { get; }
        public DateTime Hasta { get; set; }
        public decimal Monto { get; set; }
        public IEnumerable<IDetallePago> Detalles { get; }

        public RangoPago(PagoContrato padre, DateTime hasta, decimal monto, IEnumerable<IDetallePago> detalles)
        {
            Padre = padre;
            Hasta = hasta;
            Monto = monto;
            Detalles = detalles;

            var rangoPagoCambiado = from e in padre.ToObservableProperties()
                                    where e.Args.PropertyName == nameof(Padre.RangoPagoSeleccionado)
                                    select Unit.Default;

            rangoPagoCambiado.Subscribe(u => N.Change(nameof(Activo)));
        }

        public bool EsPrimeroConRestanteCero { get; set; }
        public bool NoEsPrimeroConRestanteCero => !EsPrimeroConRestanteCero;

        public decimal AdeudoRestante => Math.Max(0.0m, Padre.Contrato.Adeudo - Monto);

        public bool Activo
        {
            get { return Padre.RangoPagoSeleccionado == this; }
            set { if (value) Padre.RangoPagoSeleccionado = this; }
        }
    }
}
