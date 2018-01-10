using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Adeudos;
using AguaSB.Utilerias;
using AguaSB.Operaciones.Montos;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class PagoPorRangos : OpcionPago
    {
        public IReadOnlyCollection<PagoContrato> PagosContratos { get; }

        private PagoContrato pagoContratoSeleccionado;

        public PagoPorRangos(Usuario usuario, IReadOnlyCollection<Adeudo> adeudosContratos, ICalculadorMontos montos)
            : base(usuario, adeudosContratos, montos)
        {
            PagosContratos = adeudosContratos.Select(a => new PagoContrato(this, a, montos)).ToArray();

            if (adeudosContratos.Count == 1)
                PagoContratoSeleccionado = PagosContratos.First();
        }

        public PagoContrato PagoContratoSeleccionado
        {
            get { return pagoContratoSeleccionado; }
            set { N.Set(ref pagoContratoSeleccionado, value); }
        }

        public bool TieneContratoUnico => AdeudosContratos.Count == 1;
        public bool TieneMultiplesContratos => AdeudosContratos.Count > 1;

        public override Pago GenerarPago() => new Pago
        {
            CantidadPagada = PagoContratoSeleccionado.RangoPagoSeleccionado.Monto,
            Contrato = PagoContratoSeleccionado.Contrato.Contrato,
            Desde = Fecha.MesDe(PagoContratoSeleccionado.Contrato.UltimoPago.Hasta).AddMonths(1),
            Hasta = PagoContratoSeleccionado.RangoPagoSeleccionado.Hasta,
            FechaPago = Fecha.Ahora,
            Monto = PagoContratoSeleccionado.RangoPagoSeleccionado.Monto,
        };
    }
}
