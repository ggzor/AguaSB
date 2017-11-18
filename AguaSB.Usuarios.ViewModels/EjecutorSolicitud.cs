using System;
using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Utilerias.Solicitudes;
using AguaSB.Utilerias;

namespace AguaSB.Usuarios.ViewModels
{
    public delegate decimal CalculadorAdeudos(DateTime ultimoMesPagado, TipoContrato tipoContrato);

    public sealed class EjecutorSolicitud
    {
        public IList<ResultadoUsuario> Ejecutar(IQueryable<Usuario> valores, Solicitud solicitud, CalculadorAdeudos calculadorAdeudos)
        {
            var filtros = new Func<IQueryable<Usuario>, Solicitud, IQueryable<Usuario>>[]
            {
                FiltrarPorCalle, FiltrarPorClaseContrato, FiltrarPorFechaRegistro, FiltrarPorNombre, FiltrarPorUltimoMesPagado,
                FiltrarPorSeccion, FiltrarPorTipoContrato, FiltrarPorUltimoPago
            };

            valores = filtros.Aggregate(valores, (acc, f) => f(acc, solicitud));

            var resultados = valores.ToList().Select(u =>
            {
                var resultado = new ResultadoUsuario
                {
                    Usuario = u,
                    Contratos = u.Contratos.ToList().Select(c =>
                    {
                        var resultadoContrato = new ResultadoContrato
                        {
                            Contrato = c,
                            UltimoMesPagado = c.Pagos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault()?.Hasta
                        };

                        if (resultadoContrato.UltimoMesPagado is DateTime d)
                            resultadoContrato.Adeudo = calculadorAdeudos(d, c.TipoContrato);

                        return resultadoContrato;
                    }),
                    Domicilio = u.Contratos.FirstOrDefault()?.Domicilio,
                    UltimoPago = u.Contratos.FirstOrDefault()?.Pagos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault()?.FechaRegistro
                };

                resultado.Adeudo = resultado.Contratos.Select(_ => _.Adeudo).Sum();
                resultado.UltimoMesPagado = resultado.Contratos.Select(_ => _.UltimoMesPagado).Where(_ => _ != null).Min();

                return resultado;
            });

            return FiltrarPorAdeudo(solicitud, resultados);
        }

        private static IQueryable<Usuario> FiltrarPorNombre(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.Filtro<Igual<string>>(nameof(Usuario.NombreCompleto), out var nombre)
                            && !string.IsNullOrWhiteSpace(nombre.Valor))
            {
                var texto = Usuario.ConvertirATextoSolicitud(nombre.Valor);

                valores = from usuario in valores
                          where usuario.NombreSolicitud.Contains(texto)
                          select usuario;
            }

            return valores;
        }

        private static IQueryable<Usuario> FiltrarPorSeccion(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.Filtro<Igual<string>>(nameof(Calle.Seccion), out var seccion))
            {
                var nombreSeccion = seccion.Valor;

                valores = from usuario in valores
                          where usuario.Contratos.Any(contrato => contrato.Domicilio.Calle.Seccion.Nombre == nombreSeccion)
                          select usuario;
            }

            return valores;
        }

        private static IQueryable<Usuario> FiltrarPorCalle(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.Filtro<Igual<string>>(nameof(Domicilio.Calle), out var calle))
            {
                var nombreCalle = calle.Valor;

                valores = from usuario in valores
                          where usuario.Contratos.Any(contrato => contrato.Domicilio.Calle.Nombre == nombreCalle)
                          select usuario;
            }

            return valores;
        }

        private static IQueryable<Usuario> FiltrarPorFechaRegistro(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.Filtro<Rango<DateTime?>>(nameof(Usuario.FechaRegistro), out var registro))
            {
                if (registro.TieneInicio)
                {
                    var desde = registro.Desde;

                    if (registro.TieneFin)
                    {
                        var hasta = registro.Hasta;

                        valores = from usuario in valores
                                  where desde <= usuario.FechaRegistro && usuario.FechaRegistro <= hasta
                                  select usuario;
                    }
                    else
                    {
                        valores = from usuario in valores
                                  where desde <= usuario.FechaRegistro
                                  select usuario;
                    }
                }
                else
                {
                    if (registro.TieneFin)
                    {
                        var hasta = registro.Hasta;

                        valores = from usuario in valores
                                  where usuario.FechaRegistro <= hasta
                                  select usuario;
                    }
                }
            }

            return valores;
        }

        private static IQueryable<Usuario> FiltrarPorClaseContrato(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.Filtro<Igual<string>>(nameof(TipoContrato.ClaseContrato), out var claseContrato)
                            && Enum.GetValues(typeof(ClaseContrato))
                                .Cast<ClaseContrato?>().SingleOrDefault(_ => _.ToString() == claseContrato.Valor) is ClaseContrato valorClaseContrato)
            {
                valores = from usuario in valores
                          where usuario.Contratos.Any(contrato => contrato.TipoContrato.ClaseContrato == valorClaseContrato)
                          select usuario;
            }

            return valores;
        }

        private static IQueryable<Usuario> FiltrarPorTipoContrato(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.Filtro<Igual<string>>(nameof(Contrato.TipoContrato), out var tipoContrato))
            {
                var nombreTipoContrato = tipoContrato.Valor;

                valores = from usuario in valores
                          where usuario.Contratos.Any(contrato => contrato.TipoContrato.Nombre == nombreTipoContrato)
                          select usuario;
            }

            return valores;
        }

        private static IQueryable<Usuario> FiltrarPorUltimoPago(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.Filtro<Rango<DateTime?>>(nameof(ResultadoUsuario.UltimoPago), out var ultimoPago))
            {
                if (ultimoPago.TieneInicio)
                {
                    var desde = ultimoPago.Desde;

                    if (ultimoPago.TieneFin)
                    {
                        var hasta = ultimoPago.Hasta;

                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any(pago => desde <= pago.FechaRegistro && pago.FechaRegistro <= hasta))
                                  select usuario;
                    }
                    else
                    {
                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any(pago => desde <= pago.FechaRegistro))
                                  select usuario;
                    }
                }
                else
                {
                    if (ultimoPago.TieneFin)
                    {
                        var hasta = ultimoPago.Hasta;

                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any(pago => pago.FechaRegistro <= hasta))
                                  select usuario;
                    }
                }
            }

            return valores;
        }

        private static IQueryable<Usuario> FiltrarPorUltimoMesPagado(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.Filtro<Rango<DateTime?>>(nameof(ResultadoUsuario.UltimoMesPagado), out var ultimoMesPagado))
            {
                if (ultimoMesPagado.TieneInicio)
                {
                    var desde = Fecha.MesDe((DateTime)ultimoMesPagado.Desde);

                    if (ultimoMesPagado.TieneFin)
                    {
                        var hasta = Fecha.MesDe((DateTime)ultimoMesPagado.Hasta);

                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any())
                                  where (from contrato in usuario.Contratos
                                         where contrato.Pagos.Any()
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaRegistro).First().Hasta
                                         where desde <= pagadoHasta && pagadoHasta <= hasta
                                         select pagadoHasta).Any()
                                  select usuario;
                    }
                    else
                    {
                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any())
                                  where (from contrato in usuario.Contratos
                                         where contrato.Pagos.Any()
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaRegistro).First().Hasta
                                         where desde <= pagadoHasta
                                         select pagadoHasta).Any()
                                  select usuario;
                    }
                }
                else
                {
                    if (ultimoMesPagado.TieneFin)
                    {
                        var hasta = Fecha.MesDe((DateTime)ultimoMesPagado.Hasta);

                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any())
                                  where (from contrato in usuario.Contratos
                                         where contrato.Pagos.Any()
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaRegistro).First().Hasta
                                         where pagadoHasta <= hasta
                                         select pagadoHasta).Any()
                                  select usuario;
                    }
                }
            }

            return valores;
        }

        private static IList<ResultadoUsuario> FiltrarPorAdeudo(Solicitud solicitud, IEnumerable<ResultadoUsuario> resultados)
        {
            if (solicitud.Filtro<Rango<decimal?>>(nameof(ResultadoUsuario.Adeudo), out var adeudo))
            {
                if (adeudo.Desde is decimal desde)
                {
                    if (adeudo.Hasta is decimal hasta)
                    {
                        resultados = from usuario in resultados
                                     where desde <= usuario.Adeudo && usuario.Adeudo <= hasta
                                     select usuario;
                    }
                    else
                    {
                        resultados = from usuario in resultados
                                     where desde <= usuario.Adeudo
                                     select usuario;
                    }
                }
                else
                {
                    if (adeudo.Hasta is decimal hasta)
                    {
                        resultados = from usuario in resultados
                                     where usuario.Adeudo <= hasta
                                     select usuario;
                    }
                }
            }

            return resultados.ToList();
        }
    }
}
