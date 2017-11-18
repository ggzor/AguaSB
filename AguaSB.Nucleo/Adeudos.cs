using System;
using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    public static class Adeudos
    {
        /// <summary>
        /// IMPORTANTE: El arreglo tarifas debe estar ordenado desde la más antigua hasta la menos antigua. De lo contrario se calculara incorrectamente el adeudo.
        /// </summary>
        public static decimal Calcular(DateTime ultimoMesPagado, TipoContrato tipoContrato, Tarifa[] tarifasOrdenadas)
        {
            var primerMesAdeudo = ultimoMesPagado.AddMonths(1);
            var esteMes = Fecha.MesDe(Fecha.Ahora);

            if (primerMesAdeudo <= esteMes)
                return CalcularAdeudo(primerMesAdeudo, esteMes, tarifasOrdenadas) * tipoContrato.Multiplicador;
            else
                return 0.0m;
        }

        /// <summary>
        /// Calcula el adeudo entre los meses especificados
        /// </summary>
        private static decimal CalcularAdeudo(DateTime mesInicio, DateTime mesFinal, Tarifa[] tarifasOrdenadas)
        {
            var indiceTarifaActual = BuscarTarifaParaMes(mesInicio, tarifasOrdenadas);
            var adeudo = 0.0m;

            var mesActual = mesInicio;
            var tarifaActual = tarifasOrdenadas[indiceTarifaActual];

            while (mesActual <= mesFinal)
            {
                // Verificar si al mes actual se le debe aplicar una tarifa más reciente.
                if (indiceTarifaActual + 1 < tarifasOrdenadas.Length
                    && mesActual >= tarifasOrdenadas[indiceTarifaActual + 1].FechaRegistro)
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
