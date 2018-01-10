using System;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Montos.Implementacion;
using AguaSB.Operaciones.Pagos;
using AguaSB.Operaciones.Tarifas;

namespace AguaSB.Operaciones.Adeudos.Implementacion
{
    public class CalculadorAdeudosPorTarifaMensual : ICalculadorAdeudos
    {
        public ILocalizadorPagos Pagos { get; }
        public ILocalizadorTarifas Tarifas { get; }

        public CalculadorAdeudosPorTarifaMensual(ILocalizadorPagos pagos, ILocalizadorTarifas tarifas)
        {
            Pagos = pagos ?? throw new ArgumentNullException(nameof(pagos));
            Tarifas = tarifas ?? throw new ArgumentNullException(nameof(tarifas));
        }

        public Adeudo ObtenerAdeudo(Contrato contrato)
        {
            var ultimoPago = Pagos.UltimoDe(contrato);
            var ultimoMesPagado = ultimoPago.Hasta;
            var tarifas = Tarifas.Obtener().ToArray();

            return new Adeudo
            {
                Contrato = contrato,
                Cantidad = CalculadorMontos.Calcular(ultimoMesPagado, contrato.TipoContrato, tarifas),
                Detalles = CalculadorDetallesMonto.ObtenerDeAdeudo(ultimoMesPagado, contrato.TipoContrato, tarifas),
                UltimoPago = ultimoPago
            };
        }
    }
}
