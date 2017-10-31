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

        public ObjetoActivable<DateTime> UltimoPago { get; } = new ObjetoActivable<DateTime> { Valor = DateTime.Today.AddMinutes(-1), Formato = v => $"{v:d}" };

        public ValorRequerido<ClaseContrato?> ClaseContrato { get; } = new ValorRequerido<ClaseContrato?>();

        public ValorRequerido<TipoContrato> TipoContrato { get; } = new ValorRequerido<TipoContrato>();

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
                (nameof(UltimoPago), UltimoPago), (nameof(ClaseContrato), ClaseContrato),
                (nameof(TipoContrato), TipoContrato), (nameof(Seccion), Seccion), (nameof(Calle), Calle),
                (nameof(Adeudo), Adeudo), (nameof(Registro), Registro)
            };

            (from campo in campos
             let obs = campo.Propiedades.ToObservableProperties()
             let sub1 = obs.Subscribe(_ => N.Change(campo.Nombre))
             let sub2 = obs.Where(prop => prop.Args.PropertyName == nameof(Activable.Activo)).Subscribe(_ => N.Change(nameof(TieneFiltrosActivos)))
             select new { sub1, sub2 }).ForEach(_ => { });
        }

        public IEnumerable<Activable> Todos => new Activable[] { UltimoPago, ClaseContrato, TipoContrato, Seccion, Calle, Adeudo, Registro };

        public IEnumerable<Usuario> Aplicar(IEnumerable<Usuario> valores)
        {
            return valores;
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

        public override string ToString() => Valor?.ToString() ?? "Cualquiera";
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
