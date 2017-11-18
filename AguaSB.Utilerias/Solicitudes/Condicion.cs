using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace AguaSB.Utilerias.Solicitudes
{
    [JsonConverter(typeof(CondicionConverter))]
    public abstract class Condicion : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public Propiedad Propiedad { get; set; }

        protected Condicion()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));
        }

        #region PropertyChanged y DataErrorInfo
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        [JsonIgnore]
        public bool HasErrors => N.TieneErrores;
        public IEnumerable GetErrors(string propertyName) => N.Errores(propertyName);

        private readonly Lazy<Notificador> notificador;

        [JsonIgnore]
        protected Notificador N => notificador.Value;
        #endregion

        public abstract void Coercer();
    }

    public sealed class Igual<T> : Condicion
    {
        private const string PropiedadRequerida = "Debe seleccionar un valor";

        private T valor;

        [Required(ErrorMessage = PropiedadRequerida)]
        public T Valor
        {
            get { return valor; }
            set { N.Validate(ref valor, value); N.Change(nameof(TieneValor)); }
        }

        [JsonIgnore]
        public bool TieneValor => EqualityComparer<T>.Default.Equals(valor, default);

        public override void Coercer() { }

        public override string ToString() => $"{Propiedad} igual a {Valor?.ToString()}";
    }

    public sealed class Rango<T> : Condicion
    {
        private T desde;
        private T hasta;

        public T Desde
        {
            get { return desde; }
            set { N.Set(ref desde, value); N.Change(nameof(TieneInicio)); }
        }

        public T Hasta
        {
            get { return hasta; }
            set { N.Set(ref hasta, value); N.Change(nameof(TieneFin)); }
        }

        public override void Coercer()
        {
            if (TieneInicio && TieneFin)
                CoercerPorValor();
        }

        private void CoercerPorValor()
        {
            if (Comparer<T>.Default.Compare(hasta, desde) < 0)
            {
                T temp = desde;
                desde = hasta;
                hasta = temp;

                N.Change(nameof(Desde));
                N.Change(nameof(Hasta));
            }
        }

        private static readonly EqualityComparer<T> EqualityComparer = EqualityComparer<T>.Default;

        [JsonIgnore]
        public bool TieneInicio => !EqualityComparer.Equals(desde, default);

        [JsonIgnore]
        public bool TieneFin => !EqualityComparer.Equals(hasta, default);

        public override string ToString()
        {
            if (TieneInicio)
            {
                if (TieneFin)
                    return $"{Propiedad} desde {desde} hasta {hasta}";
                else
                    return $"{Propiedad} desde {desde}";
            }
            else
            {
                if (TieneFin)
                    return $"{Propiedad} hasta {hasta}";
                else
                    return Propiedad?.ToString();
            }
        }
    }
}
