using System;

namespace AguaSB.Utilerias.Solicitudes
{
    public static class PropiedadFechaEx
    {
        public static Rango<DateTime?> Desde(this string propiedad, DateTime valor) => new Rango<DateTime?> { Propiedad = propiedad, Desde = valor };
        public static Rango<DateTime?> Hasta(this string propiedad, DateTime valor) => new Rango<DateTime?> { Propiedad = propiedad, Hasta = valor };
        public static Rango<DateTime?> Entre(this string propiedad, DateTime desde, DateTime hasta) => new Rango<DateTime?> { Propiedad = propiedad, Desde = desde, Hasta = hasta };
    }
}
