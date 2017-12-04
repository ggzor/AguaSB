using AguaSB.Nucleo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class InformacionPago
    {
        public InformacionPago(Usuario usuario, IEnumerable<Contrato> contratos, IEnumerable<(int ConteoInicio, int ConteoFin, IEnumerable<RangoPago> Rangos)> rangosPago)
        {
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));

            if (contratos == null)
                throw new ArgumentNullException(nameof(contratos));

            Contratos = contratos.Select(c => new ContratoSeleccionable { Valor = c }).ToList();

            if (Contratos.Count == 0)
                throw new ArgumentException("Se requiere al menos un contrato para ser seleccionado.", nameof(contratos));

            Contratos[0].Activo = true;

            if (rangosPago == null)
                throw new ArgumentNullException(nameof(rangosPago));

            var pagos = rangosPago.Select(t => t.Rangos).ToArray();

            if (pagos.Any(p => p == null || !p.Any()))
                throw new ArgumentException("Cada tipo de rango de pago requiere ser no nulo y tener al menos un elemento.", nameof(rangosPago));

            RangosPago = (from columna in rangosPago
                          let primero = columna.Rangos.First()
                          let ultimo = columna.Rangos.Last()
                          let encabezado = new EncabezadoRangosPago
                          {
                              AdeudoInicio = primero.Monto,
                              AdeudoFin = ultimo.Monto,
                              MesInicio = primero.Hasta,
                              MesFin = ultimo.Hasta,
                              ConteoInicio = columna.ConteoInicio,
                              ConteoFin = columna.ConteoFin
                          }
                          let rangos = columna.Rangos.Select(r => new RangoPagoSeleccionable { Valor = r }).ToList()
                          select new ColumnaRangosPago { Encabezado = encabezado, RangosPago = rangos }
                          )
                         .ToList();

            RangosPago.First().RangosPago.First().Activo = true;
        }

        public Usuario Usuario { get; set; }

        public IList<ContratoSeleccionable> Contratos { get; set; }

        public bool TieneContratoUnico => Contratos.Count == 1;

        public bool TieneMultiplesContratos => Contratos.Count > 1;

        public IEnumerable<ColumnaRangosPago> RangosPago { get; set; }

        public Contrato ObtenerContratoSeleccionado()
        {
            if (TieneContratoUnico)
                return Contratos.Single().Valor;
            else
                return Contratos.Single(c => c.Activo).Valor;
        }

        public RangoPago ObtenerRangoPagoSeleccionado() =>
            RangosPago.Select(r => r.RangosPago)
                .Aggregate(Enumerable.Empty<RangoPagoSeleccionable>(), (l1, l2) => l1.Concat(l2))
                .Single(r => r.Activo).Valor;
    }
}
