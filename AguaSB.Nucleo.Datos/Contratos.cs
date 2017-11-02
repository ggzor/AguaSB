using AguaSB.Datos;
using System.Collections.Generic;
using System.Linq;
using System;
using AguaSB.Utilerias;

namespace AguaSB.Nucleo.Datos
{
    public static class Contratos
    {
        public static IDictionary<ClaseContrato, IList<TipoContrato>> TiposContratoAgrupados(IRepositorio<TipoContrato> tiposContratoRepo) =>
            (from contrato in tiposContratoRepo.Datos
             orderby contrato.Nombre
             group contrato by contrato.ClaseContrato into g
             orderby g.Key
             select g)
            .ToDictionary(g => g.Key, g => g.ToList() as IList<TipoContrato>);

        /// <summary>
        /// IMPORTANTE: El arreglo tarifas debe estar ordenado desde la más antigua hasta la menos antigua. De lo contrario se calculara incorrectamente el adeudo.
        /// </summary>
        public static decimal CalcularAdeudo(Contrato contrato, Tarifa[] tarifasOrdenadas)
        {
            if(contrato.Pagos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault() is Pago p && p.FechaRegistro.Year == 2016)
                Console.WriteLine("Here");
            // El mes que abarca el último pago o, si no hay pagos, el mes de la fecha de registro.
            var ultimoMes = contrato.Pagos
                .OrderByDescending(_ => _.FechaRegistro)
                .FirstOrDefault()?.Hasta ?? contrato.FechaRegistro;
            ultimoMes = MesDe(ultimoMes).AddMonths(1);

            var esteMes = MesDe(Fecha.Ahora);

            if (ultimoMes < esteMes)
            {
                var adeudo = 0.0m;
                var mesActual = ultimoMes;

                int indiceTarifa = tarifasOrdenadas.Length - 1;
                while (indiceTarifa > 0 && ultimoMes < tarifasOrdenadas[indiceTarifa].FechaRegistro)
                    indiceTarifa--;

                var tarifaActual = tarifasOrdenadas[indiceTarifa];

                while (ultimoMes < esteMes)
                {
                    if (indiceTarifa + 1 < tarifasOrdenadas.Length && ultimoMes >= tarifasOrdenadas[indiceTarifa + 1].FechaRegistro)
                    {
                        indiceTarifa++;
                        tarifaActual = tarifasOrdenadas[indiceTarifa];
                    }

                    adeudo += tarifaActual.Monto * contrato.TipoContrato.Multiplicador;

                    ultimoMes = ultimoMes.AddMonths(1);
                }

                return adeudo;
            }
            else
            {
                return 0.0m;
            }
        }

        private static DateTime MesDe(DateTime ultimoMes) => new DateTime(ultimoMes.Year, ultimoMes.Month, 01);
    }
}
