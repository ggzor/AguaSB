using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

using MoreLinq;

using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.ViewModels;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Filtros : Notificante
    {
        private bool tieneErrores;

        public bool TieneErrores
        {
            get { return tieneErrores; }
            set { N.Set(ref tieneErrores, value); }
        }

        public bool TieneFiltrosActivos => Todos.Any(f => f.Activo);

        #region Formatos
        private static readonly Func<decimal?, decimal?, string> FormatoAdeudo = (v1, v2) =>
            {
                if (v1 == null)
                {
                    if (v2 == null)
                        return "No especificado";
                    else
                        return $"Menor a {v2:C}";
                }
                else
                {
                    if (v2 == null)
                        return $"Mayor a {v1:C}";
                    else
                        return $"De {v1:C} a {v2:C}";
                }
            };

        private static readonly Func<DateTime, DateTime, string> FormatoFechas = (v1, v2) => $"Desde {v1:d} hasta {v2:d}";
        #endregion
        public ObjetoActivable<string> Texto { get; } = new ObjetoActivable<string>();

        public ObjetoActivable<DateTime> UltimoPago { get; } = new ObjetoActivable<DateTime> { Valor = DateTime.Today, Formato = v => $"{v:d}" };

        public ObjetoActivable<DateTime> PagadoHasta { get; } =
            new ObjetoActivable<DateTime> { Valor = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01), Formato = v => $"{v:MMMM yyyy}" };

        public ValorRequerido<ClaseContrato?> ClaseContrato { get; } = new ValorRequerido<ClaseContrato?>();

        public ValorRequerido<TipoContrato> TipoContrato { get; } = new ValorRequerido<TipoContrato> { Formato = t => t?.Nombre ?? "Cualquiera" };

        public ValorRequerido<Seccion> Seccion { get; } = new ValorRequerido<Seccion>();

        public ValorRequerido<Calle> Calle { get; } = new ValorRequerido<Calle>();

        public Rango<decimal?> Adeudo { get; } = new Rango<decimal?>
        {
            Formato = FormatoAdeudo
        };

        public Rango<DateTime> Registro { get; } = new Rango<DateTime> { Desde = new DateTime(2018, 01, 01), Hasta = DateTime.Today, Formato = FormatoFechas };

        public Filtros()
        {
            var campos = new(string Nombre, INotifyPropertyChanged Propiedades)[]
            {
                (nameof(UltimoPago), UltimoPago), (nameof(PagadoHasta), PagadoHasta), (nameof(ClaseContrato), ClaseContrato),
                (nameof(TipoContrato), TipoContrato), (nameof(Seccion), Seccion), (nameof(Calle), Calle),
                (nameof(Adeudo), Adeudo), (nameof(Registro), Registro)
            };

            (from campo in campos
             let obs = campo.Propiedades.ToObservableProperties()
             let sub1 = obs.Subscribe(_ => N.Change(campo.Nombre))
             let sub2 = obs.Where(prop => prop.Args.PropertyName == nameof(Activable.Activo)).Subscribe(_ => N.Change(nameof(TieneFiltrosActivos)))
             select new { sub1, sub2 }).ForEach(_ => { });
        }

        public IEnumerable<Activable> Todos => new Activable[] { Texto, UltimoPago, PagadoHasta, ClaseContrato, TipoContrato, Seccion, Calle, Adeudo, Registro };

        public IEnumerable<ResultadoUsuario> Aplicar(IQueryable<Usuario> valores, Func<DateTime, TipoContrato, decimal> calculadorAdeudos)
        {
            if (!string.IsNullOrWhiteSpace(Texto.Valor))
            {
                var texto = Usuario.ConvertirATextoSolicitud(Texto.Valor);
                valores = from usuario in valores
                          where usuario.NombreSolicitud.Contains(texto)
                          select usuario;

            }

            if (Seccion.Activo && Seccion.TieneValor)
            {
                var seccion = Seccion.Valor;
                valores = from usuario in valores
                          where usuario.Contratos.Count > 0
                          let c = usuario.Contratos.OrderByDescending(c => c.AdeudoInicial).First()
                          where c.Domicilio.Calle.Seccion == seccion
                          select usuario;
            }

            if (Calle.Activo && Calle.TieneValor)
            {
                var calle = Calle.Valor;
                valores = from usuario in valores
                          where usuario.Contratos.Count > 0
                          let c = usuario.Contratos.OrderByDescending(c => c.AdeudoInicial).First()
                          where c.Domicilio.Calle == calle
                          select usuario;
            }

            if (Registro.Activo)
            {
                var desde = Registro.Desde;
                var hasta = Registro.Hasta;
                valores = from usuario in valores
                          where desde <= usuario.FechaRegistro && usuario.FechaRegistro <= hasta
                          select usuario;
            }

            if (ClaseContrato.Activo && ClaseContrato.TieneValor)
            {
                var claseContrato = ClaseContrato.Valor;
                valores = from usuario in valores
                          where usuario.Contratos.Any(c => c.TipoContrato.ClaseContrato == claseContrato)
                          select usuario;
            }

            if (TipoContrato.Activo && TipoContrato.TieneValor)
            {
                var tipoContrato = TipoContrato.Valor;
                valores = from usuario in valores
                          where usuario.Contratos.Any(c => c.TipoContrato == tipoContrato)
                          select usuario;
            }

            if (UltimoPago.Activo)
            {
                var ultimoPago = UltimoPago.Valor;
                valores = from usuario in valores
                          where usuario.Contratos.Any(c => c.Pagos.Any(p => p.FechaRegistro >= ultimoPago))
                          select usuario;
            }

            var resultados = valores.ToList().Select(u =>
            {
                if(u.Contratos.Count > 1)
                    Console.WriteLine("Here");
                var resultado = new ResultadoUsuario
                {
                    Usuario = u,
                    Contratos = u.Contratos.Select(c =>
                    {
                        var resultadoContrato = new ResultadoContrato
                        {
                            Contrato = c,
                            PagadoHasta = c.Pagos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault()?.Hasta
                        };

                        if (resultadoContrato.PagadoHasta is DateTime d)
                            resultadoContrato.Adeudo = calculadorAdeudos(d, c.TipoContrato);

                        return resultadoContrato;
                    }),
                    Domicilio = u.Contratos.FirstOrDefault()?.Domicilio,
                    UltimoPago = u.Contratos.FirstOrDefault()?.Pagos.OrderByDescending(_ => _.FechaRegistro).FirstOrDefault()?.FechaRegistro
                };

                resultado.Adeudo = resultado.Contratos.Select(_ => _.Adeudo).Sum();
                resultado.PagadoHasta = resultado.Contratos.Select(_ => _.PagadoHasta).Where(_ => _ != null).Min();

                return resultado;
            });

            if (PagadoHasta.Activo)
            {
                var pagadoHasta = Fecha.MesDe(PagadoHasta.Valor);
                resultados = from usuario in resultados
                             where pagadoHasta <= usuario.PagadoHasta
                             select usuario;
            }

            if (Adeudo.Activo)
            {
                if (Adeudo.Desde is decimal minimo)
                {
                    if (Adeudo.Hasta is decimal maximo)
                    {
                        resultados = from usuario in resultados
                                     where minimo <= usuario.Adeudo && usuario.Adeudo <= maximo
                                     select usuario;
                    }
                    else
                    {
                        resultados = from usuario in resultados
                                     where minimo <= usuario.Adeudo
                                     select usuario;
                    }
                }
                else
                {
                    if (Adeudo.Hasta is decimal maximo)
                    {
                        resultados = from usuario in resultados
                                     where usuario.Adeudo <= maximo
                                     select usuario;
                    }
                }
            }

            return resultados;
        }
    }

    public class ValorRequerido<T> : Activable
    {
        private T valor;

        [Required(ErrorMessage = "Debe seleccionar un valor de la lista")]
        public T Valor
        {
            get { return valor; }
            set { N.Validate(ref valor, value); }
        }

        public bool TieneValor => Valor != null;

        public Func<T, string> Formato { get; set; } = v => v?.ToString() ?? "Cualquiera";

        public override string ToString() => Formato(Valor);
    }

    public class Rango<T> : Activable
    {
        private T desde;

        public T Desde
        {
            get { return desde; }
            set { N.Set(ref desde, value); }
        }

        private T hasta;

        public T Hasta
        {
            get { return hasta; }
            set { N.Set(ref hasta, value); }
        }

        public Func<T, T, string> Formato { get; set; } = (v1, v2) => $"{v1} a {v2}";

        public override string ToString() => Formato(Desde, Hasta);
    }
}
