using AguaSB.Utilerias;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Nucleo.Pagos
{
    public static class DetallesPago
    {
        public static IEnumerable<IDetallePago> Obtener(DateTime desde, DateTime hasta, TipoContrato tipoContrato, Tarifa[] tarifas)
        {
            IEnumerable<IDetallePago> AgruparPorAño(IGrouping<decimal, (DateTime Mes, decimal Monto)> grupoPorTarifa) =>
                from grupoAño in grupoPorTarifa.GroupAdjacent(p => p.Mes.Year)
                let primero = grupoAño.First()
                let ultimo = grupoAño.Last()
                select new MesesPago(primero.Mes, ultimo.Mes, grupoPorTarifa.Key);

            var gruposPorTarifa = Adeudos.CalcularMontosPorMes(desde, hasta, tipoContrato, tarifas).GroupAdjacent(p => p.Monto);

            if (gruposPorTarifa.AtLeast(2))
            {
                var gruposPorTarifaConCambio = from grupo in gruposPorTarifa.Windowed(2)
                                               let Primero = grupo.First()
                                               let Segundo = grupo.Skip(1).First()
                                               let Cambio = new CambioTarifa
                                               {
                                                   Anterior = Primero.Key,
                                                   Nueva = Segundo.Key,
                                                   Mes = Segundo.First().Mes
                                               }
                                               select (Primero, Cambio, Segundo);

                var resultados = gruposPorTarifaConCambio.TagFirstLast((g, esPrimerGrupo, esUltimoGrupo) =>
                {
                    var (primero, cambio, segundo) = g;

                    if (esPrimerGrupo && esUltimoGrupo)
                        return AgruparPorAño(primero).Concat(cambio).Concat(AgruparPorAño(segundo));
                    else if (esUltimoGrupo)
                        return AgruparPorAño(segundo);
                    else
                        return AgruparPorAño(primero).Concat(cambio);
                });

                return resultados.SelectMany(r => r);
            }
            else
            {
                return AgruparPorAño(gruposPorTarifa.First());
            }
        }


        public static IEnumerable<IDetallePago> ObtenerDeAdeudo(DateTime ultimoMesPagado, TipoContrato tipoContrato, Tarifa[] tarifas)
        {

            var primerMesAdeudo = Fecha.MesDe(ultimoMesPagado).AddMonths(1);
            var esteMes = Fecha.MesDe(Fecha.Ahora);

            if (primerMesAdeudo > esteMes)
                return Enumerable.Empty<IDetallePago>();
            else
                return Obtener(primerMesAdeudo, esteMes, tipoContrato, tarifas);
        }
    }
}