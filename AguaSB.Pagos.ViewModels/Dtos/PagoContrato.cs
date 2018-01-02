using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using MoreLinq;

using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.Nucleo.Pagos;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class PagoContrato : Notificante
    {
        public PagoPorRangos Padre { get; }
        public InformacionContrato Contrato { get; }
        public Tarifa[] Tarifas { get; }

        public IReadOnlyCollection<ColumnaRangosPago> Columnas { get; private set; }
        public IReadOnlyCollection<RangoPago> RangosPago { get; private set; }

        private RangoPago rangoPagoSeleccionado;

        #region Propiedades
        public bool Activo
        {
            get { return Padre.PagoContratoSeleccionado == this; }
            set { if (value) Padre.PagoContratoSeleccionado = this; }
        }

        public RangoPago RangoPagoSeleccionado
        {
            get { return rangoPagoSeleccionado; }
            set { N.Set(ref rangoPagoSeleccionado, value); }
        }
        #endregion

        public PagoContrato(PagoPorRangos padre, InformacionContrato contrato, Tarifa[] tarifas)
        {
            Padre = padre ?? throw new ArgumentNullException(nameof(padre));
            Contrato = contrato ?? throw new ArgumentNullException(nameof(contrato));
            Tarifas = tarifas;

            GenerarRangosPago();
            GenerarColumnas();

            var cambioSeleccionado = from e in padre.ToObservableProperties()
                                     where e.Args.PropertyName == nameof(Padre.PagoContratoSeleccionado)
                                     select Unit.Default;

            cambioSeleccionado.Subscribe(u => N.Change(nameof(Activo)));
        }

        private const int cantidadMeses = 12;
        private void GenerarRangosPago()
        {
            var primerMes = Fecha.MesDe(Contrato.UltimoPago.Hasta).AddMonths(1);
            var ultimoMes = Fecha.MesDe(Contrato.UltimoPago.Hasta).AddMonths(cantidadMeses);

            var meses = Adeudos.CalcularMontosAcumuladosPorMes(primerMes, ultimoMes, Contrato.Contrato.TipoContrato, Tarifas);

            RangosPago = meses
                .Select(m => new RangoPago(this, m.Mes, m.Monto, DetallesPago.Obtener(primerMes, m.Mes, Contrato.Contrato.TipoContrato, Tarifas).ToArray()))
                .ToArray();

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

        public bool EsUnico => Padre.Contratos.Count == 1;
        public bool TieneAdeudo => Contrato.Adeudo > 0;
        public bool NoTieneAdeudo => !TieneAdeudo;

    }
}
