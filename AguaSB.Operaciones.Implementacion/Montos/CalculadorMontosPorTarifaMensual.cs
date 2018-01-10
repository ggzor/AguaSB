using System;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Tarifas;

namespace AguaSB.Operaciones.Montos.Implementacion
{
    public class CalculadorMontosPorTarifaMensual : ICalculadorMontos
    {
        public ILocalizadorTarifas Tarifas { get; }

        public CalculadorMontosPorTarifaMensual(ILocalizadorTarifas tarifas)
        {
            Tarifas = tarifas ?? throw new ArgumentNullException(nameof(tarifas));
        }

        public Monto CalcularPara(Contrato contrato, DateTime desde, DateTime hasta)
        {
            var tarifas = Tarifas.Obtener().ToArray();

            return new Monto
            {
                Contrato = contrato,
                Cantidad = CalculadorMontos.CalcularMonto(desde, hasta, contrato.TipoContrato, tarifas),
                Detalles = CalculadorDetallesMonto.Obtener(desde, hasta, contrato.TipoContrato, tarifas)
            };
        }
    }
}
