using System;
using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    public static class Adeudos
    {
        /// <summary>
        /// IMPORTANTE: El arreglo tarifas debe estar ordenado desde la más antigua hasta la menos antigua. De lo contrario se calculara incorrectamente el adeudo.
        /// </summary>
        public static decimal Calcular(DateTime pagadoHasta, TipoContrato tipoContrato, Tarifa[] tarifasOrdenadas)
        {
            var primerMesAdeudo = pagadoHasta.AddMonths(1);
            var esteMes = Fecha.MesDe(Fecha.Ahora);

            if (primerMesAdeudo < esteMes)
                return CalcularAdeudo(primerMesAdeudo, esteMes, tarifasOrdenadas) * tipoContrato.Multiplicador;
            else
                return 0.0m;
        }

        private static decimal CalcularAdeudo(DateTime mesInicio, DateTime mesFinal, Tarifa[] tarifasOrdenadas)
        {
            var indiceTarifaActual = BuscarTarifaParaMesDeAdeudo(mesInicio, tarifasOrdenadas);
            var adeudo = 0.0m;

            var mesActual = mesInicio;
            var tarifaActual = tarifasOrdenadas[indiceTarifaActual];

            while (mesActual < mesFinal)
            {
                if (indiceTarifaActual + 1 < tarifasOrdenadas.Length &&
                    mesActual >= tarifasOrdenadas[indiceTarifaActual + 1].FechaRegistro)
                {
                    indiceTarifaActual++;
                    tarifaActual = tarifasOrdenadas[indiceTarifaActual];
                }

                adeudo += tarifaActual.Monto;

                mesActual = mesActual.AddMonths(1);
            }

            return adeudo;
        }

        /// <summary>
        /// Busca la tarifa que se va a aplicar al mes de adeudo elegido. Esta es la tarifa más antigua que se haya registrado antes del mes de adeudo.
        /// </summary>
        private static int BuscarTarifaParaMesDeAdeudo(DateTime primerMesAdeudo, Tarifa[] tarifasOrdenadas)
        {
            int indiceTarifa = tarifasOrdenadas.Length - 1;

            while (indiceTarifa > 0 && primerMesAdeudo < tarifasOrdenadas[indiceTarifa].FechaRegistro)
                indiceTarifa--;

            return indiceTarifa;
        }
    }
}
