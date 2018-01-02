using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using MoreLinq;

using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.ViewModels;
using AguaSB.Nucleo.Pagos;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class PagoContrato : Activable
    {
        public InformacionContrato Contrato { get; }
        public Tarifa[] Tarifas { get; }
        public IReadOnlyCollection<ColumnaRangosPago> Columnas { get; private set; }
        public IReadOnlyCollection<RangoPago> RangosPago { get; private set; }

        private RangoPago rangoPagoSeleccionado;

        #region Propiedades
        public RangoPago RangoPagoSeleccionado
        {
            get { return rangoPagoSeleccionado; }
            set { N.Set(ref rangoPagoSeleccionado, value); SeleccionarRangoPago(value); }
        }
        #endregion

        public PagoContrato(InformacionContrato contrato, Tarifa[] tarifas)
        {
            Contrato = contrato ?? throw new ArgumentNullException(nameof(contrato));
            Tarifas = tarifas;

            GenerarRangosPago();
            GenerarColumnas();

            var nuevosRangosPago = from evento in RangosPago.Select(r => r.ToObservableProperties()).Merge()
                                   where evento.Args.PropertyName == nameof(RangoPago.Activo)
                                   let rangoPago = (RangoPago)evento.Source
                                   where rangoPago.Activo
                                   select rangoPago;

            nuevosRangosPago.Subscribe(r => RangoPagoSeleccionado = r);
        }

        private const int cantidadMeses = 12;
        private void GenerarRangosPago()
        {
            var primerMes = Fecha.MesDe(Contrato.UltimoPago.Hasta).AddMonths(1);
            var ultimoMes = Fecha.MesDe(Contrato.UltimoPago.Hasta).AddMonths(cantidadMeses);

            var meses = Adeudos.CalcularMontosAcumuladosPorMes(primerMes, ultimoMes, Contrato.Contrato.TipoContrato, Tarifas);

            RangosPago = meses
                .Select(m => new RangoPago
                {
                    Hasta = m.Mes,
                    Monto = m.Monto,
                    AdeudoRestante = Math.Max(0, Contrato.Adeudo - m.Monto),
                    Detalles = DetallesPago.Obtener(primerMes, m.Mes, Contrato.Contrato.TipoContrato, Tarifas).ToArray()
                }).ToArray();

            DecorarPrimerRangoPagoConRestanteCero();
        }

        private void DecorarPrimerRangoPagoConRestanteCero()
        {
            if (RangosPago.First().AdeudoRestante != 0
                            && RangosPago.FirstOrDefault(r => r.AdeudoRestante == 0) is RangoPago primeroConRestanteCero)
                primeroConRestanteCero.EsPrimeroConRestanteCero = true;
        }

        private const int cantidadColumnas = 3;
        private const int rangosPorColumna = cantidadMeses / cantidadColumnas;

        private void GenerarColumnas()
        {
            Columnas = (from b in RangosPago.Batch(rangosPorColumna).Index()
                        let indice = b.Key
                        let rangos = b.Value
                        select new ColumnaRangosPago((indice * rangosPorColumna) + 1, (indice + 1) * rangosPorColumna, rangos.ToArray()))
                       .ToArray();
        }

        private void SeleccionarRangoPago(RangoPago nuevo)
        {
            if (nuevo != rangoPagoSeleccionado)
            {
                RangosPago.ForEach(r => r.Activo = false);

                nuevo.Activo = true;
            }
        }

        public bool TieneAdeudo => Contrato.Adeudo > 0;
        public bool NoTieneAdeudo => !TieneAdeudo;

    }
}
