using System.Collections.Generic;

using Newtonsoft.Json;

namespace AguaSB.Utilerias.Solicitudes
{
    [JsonConverter(typeof(CondicionConverter))]
    public abstract class Condicion
    {
        public Propiedad Propiedad { get; set; }
    }

    public sealed class Igual<T> : Condicion
    {
        public T Valor { get; set; }

        public override string ToString() => $"{Propiedad} igual a {Valor?.ToString()}";
    }

    public sealed class Rango<T> : Condicion
    {
        private T desde;
        private T hasta;

        public T Desde
        {
            get { return desde; }
            set { desde = value; Coercer(); }
        }

        public T Hasta
        {
            get { return hasta; }
            set { hasta = value; Coercer(); }
        }

        private void Coercer()
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
