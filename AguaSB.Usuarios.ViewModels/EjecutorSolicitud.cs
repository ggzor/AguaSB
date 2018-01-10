using System;
using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Utilerias.Solicitudes;
using AguaSB.Utilerias;

using AguaSB.Operaciones.Adeudos;

namespace AguaSB.Usuarios.ViewModels
{
    public delegate decimal CalculadorAdeudos(DateTime ultimoMesPagado, TipoContrato tipoContrato);

    public class EjecutorSolicitud
    {
        public Solicitud Solicitud { get; }
        public ICalculadorAdeudos CalculadorAdeudos { get; set; }

        public EjecutorSolicitud(Solicitud solicitud, ICalculadorAdeudos calculadorAdeudos)
        {
            Solicitud = solicitud ?? throw new ArgumentNullException(nameof(solicitud));
            CalculadorAdeudos = calculadorAdeudos ?? throw new ArgumentNullException(nameof(calculadorAdeudos));
        }

        public IList<ResultadoUsuario> Ejecutar(IQueryable<Usuario> valores)
        {
            var filtros = new Func<IQueryable<Usuario>, Solicitud, IQueryable<Usuario>>[]
            {
                FiltrarPorCalle, FiltrarPorClaseContrato, FiltrarPorFechaRegistro, FiltrarPorNombre, FiltrarPorUltimoMesPagado,
                FiltrarPorSeccion, FiltrarPorTipoContrato, FiltrarPorUltimoPago
            };

            valores = filtros.Aggregate(valores, (acc, f) => f(acc, Solicitud));

            return FiltrarPorAdeudo(Solicitud, ObtenerResultadosUsuario(valores));
        }

        private IEnumerable<ResultadoUsuario> ObtenerResultadosUsuario(IQueryable<Usuario> valores) =>
            ObtenerUsuariosConContratos(valores)
                .Concat(ExtraerUsuariosSinContratos(valores))
                .OrderBy(_ => _.Usuario.Id);


        private ResultadoUsuario[] ObtenerUsuariosConContratos(IQueryable<Usuario> valores)
        {
            var usuariosConContratos = from Usuario in valores
                                       where Usuario.Contratos.Any()
                                       let PrimerContrato = Usuario.Contratos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault()
                                       let Numero = PrimerContrato.Domicilio.Numero
                                       let Calle = PrimerContrato.Domicilio.Calle
                                       let Seccion = Calle.Seccion
                                       let UltimoPago = PrimerContrato.Pagos.OrderByDescending(_ => _.FechaPago).FirstOrDefault().FechaPago
                                       let DatosContratos = from Contrato in Usuario.Contratos
                                                            let Numero = Contrato.Domicilio.Numero
                                                            let Calle = Contrato.Domicilio.Calle
                                                            let Seccion = Contrato.Domicilio.Calle.Seccion
                                                            let UltimoMesPagado = Contrato.Pagos.OrderByDescending(_ => _.FechaPago).FirstOrDefault().Hasta
                                                            let UltimoPago = Contrato.Pagos.OrderByDescending(_ => _.FechaPago).FirstOrDefault().FechaPago
                                                            select new { Contrato, Numero, Calle, Seccion, UltimoPago, UltimoMesPagado, Contrato.TipoContrato }
                                       let Contactos = from Contacto in Usuario.Contactos
                                                       select new { Contacto.Informacion, Contacto.TipoContacto }
                                       select new { Usuario, Numero, Calle, Seccion, UltimoPago, DatosContratos, Contactos };

            var r = new Random();

            return usuariosConContratos.ToArray().Select(datosUsuario =>
            {
                var contratos = datosUsuario.DatosContratos.ToArray()
                    .OrderBy(_ => _.Seccion.Orden)
                    .ThenBy(_ => _.Calle.Nombre)
                    .ThenBy(_ => _.Numero)
                    .Select(datosContrato => new ResultadoContrato
                    {
                        Contrato = datosContrato.Contrato,
                        Domicilio = new Domicilio { Numero = datosContrato.Numero, Calle = new Calle { Nombre = datosContrato.Calle.Nombre, Seccion = datosContrato.Seccion } },
                        UltimoMesPagado = datosContrato.UltimoMesPagado,
                        UltimoPago = datosContrato.UltimoPago,
                        Adeudo = CalculadorAdeudos.ObtenerAdeudo(datosContrato.Contrato).Cantidad
                    })
                    .ToList();

                var resultado = new ResultadoUsuario
                {
                    Contactos = datosUsuario.Contactos.Select(datosContacto =>
                        new Contacto { Informacion = datosContacto.Informacion, TipoContacto = datosContacto.TipoContacto }).ToArray(),
                    Contratos = contratos,
                    Domicilio = new Domicilio { Numero = datosUsuario.Numero, Calle = new Calle { Nombre = datosUsuario.Calle.Nombre, Seccion = datosUsuario.Seccion } },
                    UltimoMesPagado = Fecha.MesDe(datosUsuario.DatosContratos.OrderByDescending(_ => _.UltimoMesPagado).First().UltimoMesPagado),
                    UltimoPago = datosUsuario.UltimoPago,
                    Usuario = datosUsuario.Usuario,
                    PuntosAdeudo = new[] { new PuntoAdeudo { Adeudo = r.Next(400), Fecha = Fecha.EsteMes.AddMonths(-1) }, new PuntoAdeudo { Adeudo = r.Next(200), Fecha = Fecha.EsteMes } }
                };

                resultado.Adeudo = resultado.Contratos.Select(_ => _.Adeudo).Sum();

                return resultado;
            }).ToArray();
        }

        private ResultadoUsuario[] ExtraerUsuariosSinContratos(IQueryable<Usuario> valores)
        {
            var usuariosSinContratos = from Usuario in valores
                                       where !Usuario.Contratos.Any()
                                       let Contactos = from Contacto in Usuario.Contactos
                                                       select new { Contacto.Informacion, Contacto.TipoContacto }
                                       select new { Usuario, Contactos };

            return usuariosSinContratos.ToArray().Select(datosUsuario =>
                new ResultadoUsuario
                {
                    Usuario = datosUsuario.Usuario,
                    Contactos = datosUsuario.Contactos.Select(datosContacto =>
                        new Contacto { Informacion = datosContacto.Informacion, TipoContacto = datosContacto.TipoContacto }).ToArray()
                })
            .ToArray();
        }

        private static IQueryable<Usuario> FiltrarPorNombre(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.ObtenerFiltro<Igual<string>>(nameof(Usuario.NombreCompleto), out var nombre)
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
            if (solicitud.ObtenerFiltro<Igual<string>>(nameof(Calle.Seccion), out var seccion))
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
            if (solicitud.ObtenerFiltro<Igual<string>>(nameof(Domicilio.Calle), out var calle))
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
            if (solicitud.ObtenerFiltro<Rango<DateTime?>>(nameof(Usuario.FechaRegistro), out var registro))
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
            if (solicitud.ObtenerFiltro<Igual<string>>(nameof(TipoContrato.ClaseContrato), out var claseContrato)
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
            if (solicitud.ObtenerFiltro<Igual<string>>(nameof(Contrato.TipoContrato), out var tipoContrato))
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
            if (solicitud.ObtenerFiltro<Rango<DateTime?>>(nameof(ResultadoUsuario.UltimoPago), out var ultimoPago))
            {
                if (ultimoPago.TieneInicio)
                {
                    var desde = ultimoPago.Desde;

                    if (ultimoPago.TieneFin)
                    {
                        var hasta = ultimoPago.Hasta;

                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any(pago => desde <= pago.FechaPago && pago.FechaPago <= hasta))
                                  select usuario;
                    }
                    else
                    {
                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any(pago => desde <= pago.FechaPago))
                                  select usuario;
                    }
                }
                else
                {
                    if (ultimoPago.TieneFin)
                    {
                        var hasta = ultimoPago.Hasta;

                        valores = from usuario in valores
                                  where usuario.Contratos.Any(contrato => contrato.Pagos.Any(pago => pago.FechaPago <= hasta))
                                  select usuario;
                    }
                }
            }

            return valores;
        }

        private static IQueryable<Usuario> FiltrarPorUltimoMesPagado(IQueryable<Usuario> valores, Solicitud solicitud)
        {
            if (solicitud.ObtenerFiltro<Rango<DateTime?>>(nameof(ResultadoUsuario.UltimoMesPagado), out var ultimoMesPagado))
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
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaPago).FirstOrDefault().Hasta
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
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaPago).FirstOrDefault().Hasta
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
                                         let pagadoHasta = contrato.Pagos.OrderByDescending(pago => pago.FechaPago).FirstOrDefault().Hasta
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
            if (solicitud.ObtenerFiltro<Rango<decimal?>>(nameof(ResultadoUsuario.Adeudo), out var adeudo))
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
