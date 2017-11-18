using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.Utilerias.Solicitudes;
using AguaSB.ViewModels;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Filtros : Notificante
    {
        #region Formatos
        private static readonly Func<Rango<DateTime?>, string> FormatoFechas = fecha =>
        {
            if (fecha.TieneInicio)
            {
                if (fecha.TieneFin)
                    return $"Desde {fecha.Desde:d} hasta {fecha.Hasta:d}";
                else
                    return $"Desde {fecha.Desde:d}";
            }
            else
            {
                if (fecha.TieneFin)
                    return $"Hasta {fecha.Hasta:d}";
                else
                    return "No especificado";
            }
        };

        private const string CadenaFormatoFechaMeses = "MMMM yyyy";

        private static readonly Func<Rango<DateTime?>, string> FormatoMeses = fecha =>
        {
            string F(DateTime? dateTime) => dateTime.Value.ToString(CadenaFormatoFechaMeses).Capitalizar();

            if (fecha.TieneInicio)
            {
                if (fecha.TieneFin)
                {
                    if (Fecha.MesDe((DateTime)fecha.Desde) == Fecha.MesDe((DateTime)fecha.Hasta))
                        return $"Exactamente {F(fecha.Desde).ToLower()}";
                    else
                        return $"{F(fecha.Desde)} - {F(fecha.Hasta)}";
                }
                else
                {
                    return $"{F(fecha.Desde)}";
                }
            }
            else
            {
                if (fecha.TieneFin)
                    return $"{F(fecha.Hasta)}";
                else
                    return "No especificado";
            }
        };

        private static readonly Func<Rango<decimal?>, string> FormatoAdeudo = adeudo =>
        {
            if (adeudo.TieneInicio)
            {
                if (adeudo.TieneFin)
                    return $"De {adeudo.Desde:C} a {adeudo.Hasta:C}";
                else
                    return $"Mayor a {adeudo.Desde:C}";
            }
            else
            {
                if (adeudo.TieneFin)
                    return $"Menor a {adeudo.Hasta:C}";
                else
                    return "No especificado";
            }
        };

        private static readonly Func<Igual<string>, string> FormatoTexto = texto => texto.Valor ?? "No especificado";
        #endregion

        public ObjetoActivable<Igual<string>> NombreCompleto { get; } = new ObjetoActivable<Igual<string>>
        {
            Valor = new Igual<string> { Propiedad = nameof(Usuario.NombreCompleto) },
            Formato = FormatoTexto
        };

        public ObjetoActivable<Rango<DateTime?>> UltimoPago { get; } = new ObjetoActivable<Rango<DateTime?>>
        {
            Valor = new Rango<DateTime?> { Propiedad = nameof(ResultadoUsuario.UltimoPago), Desde = Fecha.Hoy },
            Formato = FormatoFechas
        };

        public ObjetoActivable<Rango<DateTime?>> UltimoMesPagado { get; } = new ObjetoActivable<Rango<DateTime?>>
        {
            Valor = new Rango<DateTime?> { Propiedad = nameof(ResultadoUsuario.UltimoMesPagado), Desde = Fecha.EsteMes },
            Formato = FormatoMeses
        };

        public ObjetoActivable<Igual<string>> ClaseContrato { get; } = new ObjetoActivable<Igual<string>>
        {
            Valor = new Igual<string> { Propiedad = nameof(Nucleo.TipoContrato.ClaseContrato) },
            Formato = FormatoTexto
        };

        public ObjetoActivable<Igual<string>> TipoContrato { get; } = new ObjetoActivable<Igual<string>>
        {
            Valor = new Igual<string> { Propiedad = nameof(Contrato.TipoContrato) },
            Formato = FormatoTexto
        };

        public ObjetoActivable<Igual<string>> Seccion { get; } = new ObjetoActivable<Igual<string>>
        {
            Valor = new Igual<string> { Propiedad = nameof(Nucleo.Seccion) },
            Formato = FormatoTexto
        };

        public ObjetoActivable<Igual<string>> Calle { get; } = new ObjetoActivable<Igual<string>>
        {
            Valor = new Igual<string> { Propiedad = nameof(Domicilio.Calle) },
            Formato = FormatoTexto
        };

        public ObjetoActivable<Rango<decimal?>> Adeudo { get; } = new ObjetoActivable<Rango<decimal?>>
        {
            Valor = new Rango<decimal?> { Propiedad = nameof(ResultadoUsuario.Adeudo) },
            Formato = FormatoAdeudo
        };

        public ObjetoActivable<Rango<DateTime?>> FechaRegistro { get; } = new ObjetoActivable<Rango<DateTime?>>
        {
            Valor = new Rango<DateTime?> { Propiedad = nameof(Usuario.FechaRegistro), Desde = Fecha.EsteMes },
            Formato = FormatoFechas
        };

        private IEnumerable<(string Nombre, IObjetoActivable Objeto)> TodosNombrados =>
            new(string, IObjetoActivable)[]
            {
                (nameof(NombreCompleto), NombreCompleto), (nameof(UltimoPago), UltimoPago), (nameof(UltimoMesPagado), UltimoMesPagado),
                (nameof(ClaseContrato), ClaseContrato), (nameof(TipoContrato), TipoContrato), (nameof(Seccion), Seccion), (nameof(Calle), Calle),
                (nameof(Adeudo), Adeudo), (nameof(FechaRegistro), FechaRegistro)
            };

        public IEnumerable<IObjetoActivable> Todos => TodosNombrados.Select(_ => _.Objeto);

        public IEnumerable<Condicion> Activos => Todos.Where(_ => _.Activo).Select(_ => _.Valor).Cast<Condicion>();

        public bool TieneFiltrosActivos => Todos.Any(_ => _ != NombreCompleto && _.Activo);

        public Filtros()
        {
            (from filtro in TodosNombrados
             let notify = (INotifyPropertyChanged)filtro.Objeto.Valor
             let obs = notify.ToObservableProperties().Select(_ => Unit.Default)
             select (filtro.Nombre, Observable: obs))
            .ForEach(par =>
                par.Observable
                .ObserveOnDispatcher()
                .Subscribe(_ => N.Change(par.Nombre)));

            Todos.Cast<INotifyPropertyChanged>()
                .Select(_ => _.ToObservableProperties())
                .Merge()
                .Where(_ => _.Args.PropertyName == nameof(ObjetoActivable<object>.Activo))
                .ObserveOnDispatcher()
                .Subscribe(_ => N.Change(nameof(TieneFiltrosActivos)));
        }
    }
}