using System;
using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Utilerias.Solicitudes;
using AguaSB.Utilerias;

namespace AguaSB.Usuarios.ViewModels
{
    public delegate decimal CalculadorAdeudos(DateTime ultimoMesPagado, decimal multiplicador);

    public static class EjecutorSolicitud
    {
        public static IList<ResultadoUsuario> Ejecutar(IQueryable<Usuario> valores, Solicitud solicitud, CalculadorAdeudos calculadorAdeudos)
        {
            var filtros = new Func<IQueryable<Usuario>, Solicitud, IQueryable<Usuario>>[]
            {
                FiltrarPorCalle, FiltrarPorClaseContrato, FiltrarPorFechaRegistro, FiltrarPorNombre, FiltrarPorUltimoMesPagado,
                FiltrarPorSeccion, FiltrarPorTipoContrato, FiltrarPorUltimoPago
            };

            valores = filtros.Aggregate(valores, (acc, f) => f(acc, solicitud));

            var usuariosSinContratos = from Usuario in valores
                                       where !Usuario.Contratos.Any()
                                       let Contactos = from Contacto in Usuario.Contactos
                                                       select new { Contacto.Informacion, Contacto.TipoContacto }
                                       select new { Usuario, Contactos };

            var usuariosConContratos = from Usuario in valores
                                       where Usuario.Contratos.Any()
                                       let PrimerContrato = Usuario.Contratos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault()
                                       let Numero = PrimerContrato.Domicilio.Numero
                                       let Calle = PrimerContrato.Domicilio.Calle
                                       let Seccion = Calle.Seccion
                                       let UltimoPago = PrimerContrato.Pagos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault().FechaRegistro
                                       let DatosContratos = from Contrato in Usuario.Contratos
                                                            let Numero = Contrato.Domicilio.Numero
                                                            let Calle = Contrato.Domicilio.Calle
                                                            let Seccion = Contrato.Domicilio.Calle.Seccion
                                                            let UltimoMesPagado = Contrato.Pagos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault().Hasta
                                                            let UltimoPago = Contrato.Pagos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault().FechaRegistro
                                                            select new { Contrato, Numero, Calle, Seccion, UltimoPago, UltimoMesPagado, Contrato.TipoContrato }
                                       let Contactos = from Contacto in Usuario.Contactos
                                                       select new { Contacto.Informacion, Contacto.TipoContacto }
                                       select new { Usuario, Numero, Calle, Seccion, UltimoPago, DatosContratos, Contactos };

            var resultadosSinContratos = usuariosSinContratos.ToArray().Select(datosUsuario =>
                new ResultadoUsuario
                {
                    Usuario = datosUsuario.Usuario,
                    Contactos = datosUsuario.Contactos.Select(datosContacto =>
                        new Contacto { Informacion = datosContacto.Informacion, TipoContacto = datosContacto.TipoContacto }).ToArray()
                })
            .ToArray();

            var resultadosConContratos = usuariosConContratos.ToArray().Select(datosUsuario =>
            {
                var resultado = new ResultadoUsuario
                {
                    Contactos = datosUsuario.Contactos.Select(datosContacto =>
                        new Contacto { Informacion = datosContacto.Informacion, TipoContacto = datosContacto.TipoContacto }).ToArray(),
                    Contratos = datosUsuario.DatosContratos.ToArray()
                        .OrderBy(_ => _.Seccion.Orden)
                        .ThenBy(_ => _.Calle.Nombre)
                        .ThenBy(_ => _.Numero)
                        .Select(datosContrato => new ResultadoContrato
                        {
                            Contrato = datosContrato.Contrato,
                            Domicilio = new Domicilio { Numero = datosUsuario.Numero, Calle = new Calle { Nombre = datosUsuario.Calle.Nombre, Seccion = datosUsuario.Seccion } },
                            UltimoMesPagado = datosContrato.UltimoMesPagado,
                            UltimoPago = datosContrato.UltimoPago,
                            Adeudo = calculadorAdeudos(datosContrato.UltimoMesPagado, datosContrato.TipoContrato.Multiplicador)
                        }),
                    Domicilio = new Domicilio { Numero = datosUsuario.Numero, Calle = new Calle { Nombre = datosUsuario.Calle.Nombre, Seccion = datosUsuario.Seccion } },
                    UltimoMesPagado = Fecha.MesDe(datosUsuario.DatosContratos.OrderByDescending(_ => _.UltimoMesPagado).First().UltimoMesPagado),
                    UltimoPago = datosUsuario.UltimoPago,
                    Usuario = datosUsuario.Usuario
                };

                resultado.Adeudo = resultado.Contratos.Select(_ => _.Adeudo).Sum();

                return resultado;
            }).ToArray();

            var resultados = resultadosConContratos.Concat(resultadosSinContratos).OrderBy(_ => _.Usuario.Id);

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
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaRegistro).FirstOrDefault().Hasta
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
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaRegistro).FirstOrDefault().Hasta
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
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaRegistro).FirstOrDefault().Hasta
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
