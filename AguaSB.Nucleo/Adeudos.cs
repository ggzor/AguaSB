using System;
using AguaSB.Utilerias;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Nucleo
{
    public static class Adeudos
    {
        public static decimal Calcular(DateTime ultimoMesPagado, TipoContrato tipoContrato, Tarifa[] tarifasOrdenadas)
        {
            var primerMesAdeudo = Fecha.MesDe(ultimoMesPagado).AddMonths(1);
            var esteMes = Fecha.MesDe(Fecha.Ahora);

            if (primerMesAdeudo > esteMes)
                return 0.0m;
            else
                return CalcularMonto(primerMesAdeudo, esteMes, tipoContrato, tarifasOrdenadas);
        }

        public static decimal CalcularMonto(DateTime desde, DateTime hasta, TipoContrato tipoContrato, Tarifa[] tarifasOrdenadas)
        {
            desde = Fecha.MesDe(desde);
            hasta = Fecha.MesDe(hasta);

            if (desde > hasta)
                throw new ArgumentException("La fecha de inicio debe ser menor o igual a la fecha de fin.");

            return CalcularMontosAcumuladosPorMes(desde, hasta, tipoContrato, tarifasOrdenadas).Last().Monto;
        }

        public static IEnumerable<(DateTime Mes, decimal Monto)> CalcularMontosAcumuladosPorMes(DateTime desde, DateTime hasta, TipoContrato tipoContrato, Tarifa[] tarifasOrdenadas)
        {
            var montoActual = 0m;

            foreach (var (mes, monto) in CalcularMontosPorMes(desde, hasta, tipoContrato, tarifasOrdenadas))
            {
                montoActual += monto;

                yield return (mes, montoActual);
            }
        }

        public static IEnumerable<(DateTime Mes, decimal Monto)> CalcularMontosPorMes(DateTime desde, DateTime hasta, TipoContrato tipoContrato, Tarifa[] tarifasOrdenadas)
        {
            if (tarifasOrdenadas == null)
                throw new ArgumentNullException(nameof(tarifasOrdenadas));

            if (tipoContrato == null)
                throw new ArgumentNullException(nameof(tipoContrato));

            if (tarifasOrdenadas.Count() == 0)
                throw new ArgumentException("Debe haber al menos una tarifa.", nameof(tarifasOrdenadas));

            desde = Fecha.MesDe(desde);
            hasta = Fecha.MesDe(hasta);

            var indiceTarifaActual = BuscarIndiceTarifaParaMes(desde, tarifasOrdenadas);
            var tarifaActual = tarifasOrdenadas[indiceTarifaActual];

            var actual = desde;
            while (actual <= hasta)
            {
                // Intentar tomar la tarifa siguiente si: Hay otra y el mes actual está durante el periodo de la siguiente tarifa.
                var indiceTarifaSiguiente = indiceTarifaActual + 1;
                if (indiceTarifaSiguiente < tarifasOrdenadas.Length)
                {
                    var tarifaSiguiente = tarifasOrdenadas[indiceTarifaSiguiente];

                    if (actual >= tarifaSiguiente.FechaRegistro)
                    {
                        indiceTarifaActual = indiceTarifaSiguiente;
                        tarifaActual = tarifaSiguiente;
                    }
                }

                yield return (actual, tarifaActual.Monto * tipoContrato.Multiplicador);

                actual = actual.AddMonths(1);
            }
        }

        /// <summary>
        /// Busca la tarifa más antigua que se haya registrado antes o durante el mes especificado.
        /// </summary>
        private static int BuscarIndiceTarifaParaMes(DateTime mes, Tarifa[] tarifasOrdenadas)
        {
            int indiceTarifa = tarifasOrdenadas.Length - 1;

            while (indiceTarifa > 0 && mes < tarifasOrdenadas[indiceTarifa].FechaRegistro)
                indiceTarifa--;

            return indiceTarifa;
        }
    }
}
