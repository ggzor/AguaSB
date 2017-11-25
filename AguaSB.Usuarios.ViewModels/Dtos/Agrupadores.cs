using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.ViewModels;
using System;
using System.Collections.Generic;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public static class Agrupadores
    {
        private static readonly Seccion SinSeccion = new Seccion { Nombre = "Sin domicilios registrados", Orden = -1 };

        public static readonly Agrupador<Seccion> Seccion = new Agrupador<Seccion>
        {
            Nombre = "Sección",
            Propiedad = nameof(Seccion),
            SelectorClave = _ => _?.Domicilio?.Calle?.Seccion ?? SinSeccion,
            SelectorNombre = _ => _.Nombre,
            Comparador = new FuncComparer<Seccion>((s1, s2) => s1.Orden.CompareTo(s2.Orden))
        };

        public static readonly Agrupador<string> Calle = new Agrupador<string>
        {
            Nombre = nameof(Calle),
            Propiedad = nameof(Calle),
            SelectorClave = _ => _?.Domicilio?.Calle?.Nombre,
            SelectorNombre = _ => _ ?? "Sin domicilios registrados",
            Comparador = Comparer<string>.Default
        };

        public static readonly Agrupador<(decimal, decimal)> Adeudo = new Agrupador<(decimal, decimal)>
        {
            Nombre = nameof(Adeudo),
            Propiedad = nameof(ResultadoUsuario.Adeudo),
            SelectorClave = _ => ClasificarAdeudo(_.Adeudo),
            SelectorNombre = _ => SeleccionarNombreAdeudo(_),
            Comparador = new FuncComparer<(decimal, decimal)>((a, b) => a.Item2.CompareTo(b.Item2))
        };

        public static readonly Agrupador<DateTime?> UltimoMesPagado = new Agrupador<DateTime?>
        {
            Nombre = "Último mes pagado",
            Propiedad = nameof(ResultadoUsuario.UltimoMesPagado),
            SelectorClave = _ => _.UltimoMesPagado,
            SelectorNombre = _ => _?.ToString("MMMM yyyy").Capitalizar() ?? "Sin pagos",
            Comparador = Comparer<DateTime?>.Default
        };

        public static readonly Agrupador<DateTime> FechaRegistro = new Agrupador<DateTime>
        {
            Nombre = "Fecha de registro",
            Propiedad = nameof(Usuario.FechaRegistro),
            SelectorClave = _ => Fecha.MesDe(_.Usuario.FechaRegistro),
            SelectorNombre = _ => _.ToString("MMMM yyyy").Capitalizar(),
            Comparador = Comparer<DateTime>.Default
        };

        private static (decimal, decimal) ClasificarAdeudo(decimal adeudo)
        {
            if (adeudo == 0)
                return (0, 0);

            if (adeudo <= 200)
                return (0, 200);

            if (adeudo <= 500)
                return (200, 500);

            int valor;

            if (adeudo <= 2000)
                valor = 500;
            else
                valor = 1000;

            int rango = (int)adeudo / valor;

            return (rango * valor, (rango + 1) * valor);
        }

        private static string SeleccionarNombreAdeudo((decimal Desde, decimal Hasta) rango)
        {
            if (rango.Hasta == 0)
                return "Al corriente";
            else
                return $"{rango.Desde:C} - {rango.Hasta:C}";
        }

        public static IEnumerable<IAgrupador> Todos => new IAgrupador[]
        {
            Seccion, Calle, Adeudo, UltimoMesPagado, FechaRegistro
        };
    }
}
