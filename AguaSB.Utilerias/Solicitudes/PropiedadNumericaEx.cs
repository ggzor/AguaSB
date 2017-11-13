namespace AguaSB.Utilerias.Solicitudes
{
    public static class PropiedadNumericaEx
    {
        public static Rango<decimal?> Desde(this Propiedad propiedad, int desde) => new Rango<decimal?> { Propiedad = propiedad, Desde = desde };
        public static Rango<decimal?> Desde(this Propiedad propiedad, double desde) => new Rango<decimal?> { Propiedad = propiedad, Desde = (decimal?)desde };
        public static Rango<decimal?> Desde(this Propiedad propiedad, decimal desde) => new Rango<decimal?> { Propiedad = propiedad, Desde = desde };

        public static Rango<decimal?> Hasta(this Propiedad propiedad, int hasta) => new Rango<decimal?> { Propiedad = propiedad, Hasta = hasta };
        public static Rango<decimal?> Hasta(this Propiedad propiedad, double hasta) => new Rango<decimal?> { Propiedad = propiedad, Hasta = (decimal?)hasta };
        public static Rango<decimal?> Hasta(this Propiedad propiedad, decimal hasta) => new Rango<decimal?> { Propiedad = propiedad, Hasta = hasta };

        public static Rango<decimal?> Entre(this Propiedad propiedad, int desde, int hasta) => new Rango<decimal?> { Propiedad = propiedad, Desde = desde, Hasta = hasta };
        public static Rango<decimal?> Entre(this Propiedad propiedad, double desde, double hasta) => new Rango<decimal?> { Propiedad = propiedad, Desde = (decimal?)desde, Hasta = (decimal?)hasta };
        public static Rango<decimal?> Entre(this Propiedad propiedad, decimal desde, decimal hasta) => new Rango<decimal?> { Propiedad = propiedad, Desde = desde, Hasta = hasta };

        public static Rango<decimal?> Desde(this string propiedad, int desde) => new Rango<decimal?> { Propiedad = propiedad, Desde = desde };
        public static Rango<decimal?> Desde(this string propiedad, double desde) => new Rango<decimal?> { Propiedad = propiedad, Desde = (decimal?)desde };
        public static Rango<decimal?> Desde(this string propiedad, decimal desde) => new Rango<decimal?> { Propiedad = propiedad, Desde = desde };

        public static Rango<decimal?> Hasta(this string propiedad, int hasta) => new Rango<decimal?> { Propiedad = propiedad, Hasta = hasta };
        public static Rango<decimal?> Hasta(this string propiedad, double hasta) => new Rango<decimal?> { Propiedad = propiedad, Hasta = (decimal?)hasta };
        public static Rango<decimal?> Hasta(this string propiedad, decimal hasta) => new Rango<decimal?> { Propiedad = propiedad, Hasta = hasta };

        public static Rango<decimal?> Entre(this string propiedad, int desde, int hasta) => new Rango<decimal?> { Propiedad = propiedad, Desde = desde, Hasta = hasta };
        public static Rango<decimal?> Entre(this string propiedad, double desde, double hasta) => new Rango<decimal?> { Propiedad = propiedad, Desde = (decimal?)desde, Hasta = (decimal?)hasta };
        public static Rango<decimal?> Entre(this string propiedad, decimal desde, decimal hasta) => new Rango<decimal?> { Propiedad = propiedad, Desde = desde, Hasta = hasta };
    }
}
