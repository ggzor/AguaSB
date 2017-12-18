using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using MoreLinq;

using AguaSB.Nucleo;
using AguaSB.Utilerias;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class PagoPorRangos : OpcionPago
    {
        public IReadOnlyCollection<PagoContrato> PagosContratos { get; }

        private PagoContrato pagoContratoSeleccionado;

        public PagoPorRangos(Usuario usuario, IReadOnlyCollection<InformacionContrato> contratos, Tarifa[] tarifas)
            : base(usuario, contratos, tarifas)
        {
            PagosContratos = contratos.Select(c => new PagoContrato(c, tarifas)).ToArray();

            if (contratos.Count == 1)
                PagoContratoSeleccionado = PagosContratos.First();

            var nuevosActivos = from evento in PagosContratos.Select(p => p.ToObservableProperties()).Merge()
                                where evento.Args.PropertyName == nameof(PagoContrato.Activo)
                                let pagoContrato = (PagoContrato)evento.Source
                                where pagoContrato.Activo
                                select pagoContrato;

            nuevosActivos.Subscribe(n => PagoContratoSeleccionado = n);
        }

        public PagoContrato PagoContratoSeleccionado
        {
            get { return pagoContratoSeleccionado; }
            set { N.Set(ref pagoContratoSeleccionado, value); SeleccionarContrato(value); }
        }

        private void SeleccionarContrato(PagoContrato nuevo)
        {
            if (nuevo != pagoContratoSeleccionado)
            {
                PagosContratos.ForEach(c => c.Activo = false);

                nuevo.Activo = true;
            }
        }

        public bool TieneContratoUnico => Contratos.Count == 1;

        public bool TieneMultiplesContratos => Contratos.Count > 1;

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
