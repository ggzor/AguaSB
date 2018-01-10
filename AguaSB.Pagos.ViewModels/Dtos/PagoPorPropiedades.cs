using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using static AguaSB.Utilerias.Fecha;
using AguaSB.Nucleo;
using AguaSB.Operaciones.Adeudos;
using AguaSB.Operaciones.Montos;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class PagoPorPropiedades : OpcionPago
    {
        private Adeudo? contrato;
        private DateTime pagarHasta;
        private decimal monto;
        private decimal cantidadPagada;
        private DateTime fechaPago;

        #region Propiedades
        [Required(ErrorMessage = "Debe especificar un contrato")]
        public Adeudo? Contrato
        {
            get { return contrato; }
            set { N.Validate(ref contrato, value); ActualizarInformacionContrato(); }
        }

        [CustomValidation(typeof(PagoPorPropiedades), nameof(ValidarUltimoMesPagado))]
        public DateTime PagarHasta
        {
            get { return pagarHasta; }
            set { N.Validate(ref pagarHasta, value); ActualizarInformacionPago(); }
        }

        public decimal Monto
        {
            get { return monto; }
            set { N.Set(ref monto, value); N.Change(nameof(AdeudoRestante), nameof(DetallesMonto)); }
        }

        public IEnumerable<IDetalleMonto> DetallesMonto { get; private set; }

        public decimal AdeudoRestante => Math.Max(0, (Contrato?.Cantidad ?? 0) - Monto);

        [Range(typeof(decimal), "0", "1000000", ErrorMessage = "La cantidad pagada debe ser mayor o igual a $0.00")]
        public decimal CantidadPagada
        {
            get { return cantidadPagada; }
            set { N.Validate(ref cantidadPagada, value); }
        }

        [CustomValidation(typeof(PagoPorPropiedades), nameof(ValidarFechaPago))]
        public DateTime FechaPago
        {
            get { return fechaPago; }
            set { N.Validate(ref fechaPago, EstablecerHoraActual(value)); }
        }
        #endregion

        public PagoPorPropiedades(Usuario usuario, IReadOnlyCollection<Adeudo> contratos, ICalculadorMontos montos)
            : base(usuario, contratos, montos)
        {
            Contrato = contratos.First();
        }

        private void ActualizarInformacionContrato()
        {
            if (Contrato != null)
            {
                PagarHasta = MesDe((DateTime)Contrato?.UltimoPago.Hasta).AddMonths(1);
                FechaPago = Ahora;

                ActualizarInformacionPago();
            }
        }

        private void ActualizarInformacionPago()
        {
            var primerMes = MesDe((DateTime)Contrato?.UltimoPago.Hasta).AddMonths(1);

            if (primerMes <= PagarHasta)
            {
                var monto = Montos.CalcularPara(Contrato?.Contrato, primerMes, PagarHasta);
                DetallesMonto = monto.Detalles;
                Monto = monto.Cantidad;
            }
            else
            {
                DetallesMonto = null;
                Monto = 0;
            }

            CantidadPagada = Monto;
        }

        public override Pago GenerarPago() => new Pago
        {
            Contrato = Contrato?.Contrato,
            Desde = MesDe((DateTime)Contrato?.UltimoPago.Hasta.AddMonths(1)),
            Hasta = MesDe(PagarHasta),
            FechaPago = FechaPago,
            Monto = Monto,
            CantidadPagada = CantidadPagada
        };

        #region Validacion Fechas
        public static ValidationResult ValidarUltimoMesPagado(DateTime fecha, ValidationContext contexto)
        {
            if (contexto.ObjectInstance is PagoPorPropiedades p && p.Contrato?.UltimoPago?.Hasta is DateTime ultimoMesPagado)
            {
                fecha = MesDe(fecha);
                ultimoMesPagado = MesDe(ultimoMesPagado);

                if (fecha <= ultimoMesPagado)
                    return new ValidationResult("Debe ser después del último mes pagado");
                else
                    return ValidationResult.Success;
            }
            else
            {
                throw new ArgumentException($"Sólo se puede usar este metodo dentro de la clase {typeof(PagoPorPropiedades)}", nameof(contexto));
            }
        }

        public static ValidationResult ValidarFechaPago(DateTime fecha, ValidationContext contexto)
        {
            if (contexto.ObjectInstance is PagoPorPropiedades p && p.Contrato?.UltimoPago?.FechaPago is DateTime ultimaFechaPago)
            {
                if (fecha <= ultimaFechaPago)
                    return new ValidationResult("Debe ser después de la fecha del último pago");
                else
                    return ValidationResult.Success;
            }
            else
            {
                throw new ArgumentException($"Sólo se puede usar este metodo dentro de la clase {typeof(PagoPorPropiedades)}", nameof(contexto));
            }
        }
        #endregion
    }
}
