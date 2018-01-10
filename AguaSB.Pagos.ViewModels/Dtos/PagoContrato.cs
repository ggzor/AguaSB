using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using MoreLinq;

using AguaSB.Utilerias;

using AguaSB.Operaciones.Adeudos;
using AguaSB.Operaciones.Montos;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class PagoContrato : Notificante
    {
        public PagoPorRangos Padre { get; }
        public Adeudo Contrato { get; }
        public ICalculadorMontos Montos { get; }

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

        public PagoContrato(PagoPorRangos padre, Adeudo contrato, ICalculadorMontos montos)
        {
            Padre = padre ?? throw new ArgumentNullException(nameof(padre));
            Contrato = contrato;
            Montos = montos ?? throw new ArgumentNullException(nameof(montos));
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

            var rangos = from dm in Enumerable.Range(0, cantidadMeses)
                         let mes = primerMes.AddMonths(dm)
                         let monto = Montos.CalcularPara(Contrato.Contrato, primerMes, mes)
                         select new RangoPago(this, mes, monto.Cantidad, monto.Detalles);

            RangosPago = rangos.ToArray();

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

        public bool EsUnico => Padre.AdeudosContratos.Count == 1;
    }
}
