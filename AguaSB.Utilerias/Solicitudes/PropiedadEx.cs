namespace AguaSB.Utilerias.Solicitudes
{
    public static class PropiedadEx
    {
        public static Igual<T> Igual<T>(this Propiedad propiedad, T valor) => new Igual<T> { Propiedad = propiedad, Valor = valor };
        public static Rango<T> Desde<T>(this Propiedad propiedad, T desde) => new Rango<T> { Propiedad = propiedad, Desde = desde };
        public static Rango<T> Hasta<T>(this Propiedad propiedad, T hasta) => new Rango<T> { Propiedad = propiedad, Hasta = hasta };
        public static Rango<T> Entre<T>(this Propiedad propiedad, T desde, T hasta) => new Rango<T> { Propiedad = propiedad, Desde = desde, Hasta = hasta };

        public static Igual<T> Igual<T>(this string propiedad, T valor) => new Igual<T> { Propiedad = propiedad, Valor = valor };
        public static Rango<T> Desde<T>(this string propiedad, T desde) => new Rango<T> { Propiedad = propiedad, Desde = desde };
        public static Rango<T> Hasta<T>(this string propiedad, T hasta) => new Rango<T> { Propiedad = propiedad, Hasta = hasta };
        public static Rango<T> Entre<T>(this string propiedad, T desde, T hasta) => new Rango<T> { Propiedad = propiedad, Desde = desde, Hasta = hasta };
    }
}
