using System;
using System.Collections.Generic;
using System.Linq;

using MoreLinq;

using AguaSB.Nucleo;
using AguaSB.Utilerias;

namespace AguaSB.Operaciones.Montos.Implementacion
{
    public static class CalculadorDetallesMonto
    {
        public static IEnumerable<IDetalleMonto> Obtener(DateTime desde, DateTime hasta, TipoContrato tipoContrato, Tarifa[] tarifas)
        {
            IEnumerable<IDetalleMonto> AgruparPorAño(IGrouping<decimal, (DateTime Mes, decimal Monto)> grupoPorTarifa) =>
                from grupoAño in grupoPorTarifa.GroupAdjacent(p => p.Mes.Year)
                let primero = grupoAño.First()
                let ultimo = grupoAño.Last()
                select new MesesMonto(primero.Mes, ultimo.Mes, grupoPorTarifa.Key);

            var gruposPorTarifa = CalculadorMontos.CalcularMontosPorMes(desde, hasta, tipoContrato, tarifas).GroupAdjacent(p => p.Monto);

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


        public static IEnumerable<IDetalleMonto> ObtenerDeAdeudo(DateTime ultimoMesPagado, TipoContrato tipoContrato, Tarifa[] tarifas)
        {

            var primerMesAdeudo = Fecha.MesDe(ultimoMesPagado).AddMonths(1);
            var ultimoMesCalculado = Fecha.MesDe(Fecha.Ahora).AddMonths(-1);

            if (primerMesAdeudo > ultimoMesCalculado)
                return Enumerable.Empty<IDetalleMonto>();
            else
                return Obtener(primerMesAdeudo, ultimoMesCalculado, tipoContrato, tarifas);
        }
    }
}
