using System;
using AguaSB.Utilerias;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Nucleo
{
    public static class Adeudos
    {
        /// <summary>
        /// Calcula los adeudos por cada mes desde el mes de inicio especificado, considerando los pagos especificados.
        /// </summary>
        /// <param name="inicio">Mes de inicio (Inclusivo)</param>
        /// <param name="multiplicador">El multiplicador a aplicar por cada monto de las tarifas.</param>
        /// <param name="tarifasOrdenadas">Las tarifas ordenadas ascendentemente por fecha de registro.</param>
        /// <param name="pagosOrdenados">Los pagos ordenados ascendentemente por fecha de registro.</param>
        /// <returns></returns>
        /*public static IEnumerable<(DateTime Mes, decimal Adeudo)> CalcularConPagos(DateTime inicio, decimal multiplicador, Tarifa[] tarifasOrdenadas, Pago[] pagosOrdenados)
        {
            if (inicio > Fecha.EsteMes)
                throw new ArgumentOutOfRangeException("La fecha de inicio debe ser menor que este mes.");

            var conteoPagos = pagosOrdenados.Length;
            int indicePagoActual = -1;

            foreach (var (mes, adeudo) in CalcularAdeudosAcumulados(inicio, Fecha.EsteMes, tarifasOrdenadas))
            {

            }
        }*/

        /// <summary>
        /// IMPORTANTE: El arreglo tarifas debe estar ordenado desde la más antigua hasta la menos antigua. De lo contrario se calculara incorrectamente el adeudo.
        /// </summary>
        public static decimal Calcular(DateTime ultimoMesPagado, decimal multiplicador, Tarifa[] tarifasOrdenadas)
        {
            var primerMesAdeudo = ultimoMesPagado.AddMonths(1);
            var esteMes = Fecha.MesDe(Fecha.Ahora);

            if (primerMesAdeudo <= esteMes)
                return CalcularAdeudosAcumulados(primerMesAdeudo, esteMes, tarifasOrdenadas).Select(_ => _.Adeudo).Sum() * multiplicador;
            else
                return 0.0m;
        }

        /// <summary>
        /// Calcula el adeudo entre dos fechas de acuerdo a las tarifas especificadas.
        /// </summary>
        /// <param name="mesInicio">El primer mes a considerar</param>
        /// <param name="mesFin">El último mes a considerar (Inclusivo)</param>
        /// <param name="tarifasOrdenadas">Las tarifas a considerar para el calculo de adeudos. Debe estar en orden ascendente por fecha de registro.</param>
        /// <returns>Un arreglo de parejas que representa cada mes y su adeudo.</returns>
        private static IEnumerable<(DateTime Mes, decimal Adeudo)> CalcularAdeudosAcumulados(DateTime mesInicio, DateTime mesFin, Tarifa[] tarifasOrdenadas)
        {
            mesInicio = Fecha.MesDe(mesInicio);
            mesFin = Fecha.MesDe(mesFin);

            if (mesInicio > mesFin)
                throw new ArgumentOutOfRangeException("El mes de inicio debe ser menor o igual que el mes final.");

            var conteoTarifas = tarifasOrdenadas.Length;

            if (conteoTarifas == 0)
                throw new ArgumentException("Debe haber al menos una tarifa", nameof(tarifasOrdenadas));

            var indiceTarifaActual = BuscarTarifaParaMes(mesInicio, tarifasOrdenadas);
            var tarifaActual = tarifasOrdenadas[indiceTarifaActual];

            var mesActual = mesInicio;

            while (mesActual <= mesFin)
            {
                // Intentar tomar la tarifa siguiente si: Hay otra y el mes actual está durante el periodo de esta tarifa.
                var indiceTarifaSiguiente = indiceTarifaActual + 1;
                if (indiceTarifaSiguiente < conteoTarifas)
                {
                    var tarifaSiguiente = tarifasOrdenadas[indiceTarifaSiguiente];

                    if (mesActual >= tarifaSiguiente.FechaRegistro)
                    {
                        indiceTarifaActual = indiceTarifaSiguiente;
                        tarifaActual = tarifaSiguiente;
                    }
                }

                mesActual = mesActual.AddMonths(1);

                yield return (mesActual, tarifaActual.Monto);
            }
        }

        /// <summary>
        /// Busca la tarifa más antigua que se haya registrado antes o durante el mes especificado.
        /// </summary>
        private static int BuscarTarifaParaMes(DateTime mes, Tarifa[] tarifasOrdenadas)
        {
            int indiceTarifa = tarifasOrdenadas.Length - 1;

            while (indiceTarifa > 0 && mes < tarifasOrdenadas[indiceTarifa].FechaRegistro)
                indiceTarifa--;

            return indiceTarifa;
        }
    }
}
