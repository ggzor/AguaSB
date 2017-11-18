using AguaSB.Nucleo;
using AguaSB.Utilerias.Solicitudes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public interface IOrdenamiento
    {
        string Nombre { get; set; }

        Propiedad Propiedad { get; set; }

        ListSortDirection? Direccion { get; set; }

        IOrderedEnumerable<ResultadoUsuario> Ordenar(IEnumerable<ResultadoUsuario> elementos);

        void Cambiar();
    }

    public class Ordenamiento<T> : IOrdenamiento
    {
        public string Nombre { get; set; }

        public Propiedad Propiedad { get; set; }

        public ListSortDirection? Direccion { get; set; }

        public Func<ResultadoUsuario, T> SelectorOrden { get; set; }

        public IOrderedEnumerable<ResultadoUsuario> Ordenar(IEnumerable<ResultadoUsuario> elementos)
        {
            if (Direccion == ListSortDirection.Ascending)
                return elementos.OrderBy(SelectorOrden);
            else
                return elementos.OrderByDescending(SelectorOrden);
        }

        public void Cambiar()
        {
            switch (Direccion)
            {
                case ListSortDirection.Ascending:
                    Direccion = ListSortDirection.Descending;
                    break;
                case ListSortDirection.Descending:
                    Direccion = null;
                    break;
                default:
                    Direccion = ListSortDirection.Ascending;
                    break;
            }
        }

        public override string ToString() => Nombre;
    }

    public class Ordenamientos
    {
        public Ordenamiento<int> Id { get; } = new Ordenamiento<int>
        {
            Nombre = nameof(Id),
            Propiedad = nameof(Usuario.Id),
            SelectorOrden = _ => _.Usuario.Id
        };

        public Ordenamiento<string> NombreCompleto { get; } = new Ordenamiento<string>
        {
            Nombre = "Nombre",
            Propiedad = nameof(Usuario.NombreCompleto),
            SelectorOrden = _ => _.Usuario.NombreCompleto
        };

        public Ordenamiento<decimal> Adeudo { get; } = new Ordenamiento<decimal>
        {
            Nombre = nameof(Adeudo),
            Propiedad = $"{nameof(ResultadoUsuario.Adeudo)}",
            SelectorOrden = _ => _.Adeudo
        };

        public Ordenamiento<DateTime?> UltimoMesPagado { get; } = new Ordenamiento<DateTime?>
        {
            Nombre = "Último mes pagado",
            Propiedad = $"{nameof(ResultadoUsuario.UltimoMesPagado)}",
            SelectorOrden = _ => _.UltimoMesPagado
        };

        public Ordenamiento<int> Seccion { get; } = new Ordenamiento<int>
        {
            Nombre = "Sección",
            Propiedad = nameof(Nucleo.Calle.Seccion),
            SelectorOrden = _ => _.Domicilio.Calle.Seccion.Orden
        };

        public Ordenamiento<string> Calle { get; } = new Ordenamiento<string>
        {
            Nombre = nameof(Calle),
            Propiedad = nameof(Nucleo.Calle.Nombre),
            SelectorOrden = _ => _.Domicilio.Calle.Nombre
        };

        public Ordenamiento<string> Numero { get; } = new Ordenamiento<string>
        {
            Nombre = "Número",
            Propiedad = nameof(Domicilio.Numero),
            SelectorOrden = _ => _.Domicilio.Numero
        };

        public Ordenamiento<DateTime?> UltimoPago { get; } = new Ordenamiento<DateTime?>
        {
            Nombre = "Último pago",
            Propiedad = nameof(ResultadoUsuario.UltimoPago),
            SelectorOrden = _ => _.UltimoPago
        };

        public Ordenamiento<DateTime?> FechaRegistro { get; } = new Ordenamiento<DateTime?>
        {
            Nombre = "Fecha de registro",
            Propiedad = nameof(Usuario.FechaRegistro),
            SelectorOrden = _ => _.Usuario.FechaRegistro
        };

        public IEnumerable<IOrdenamiento> Todos => new IOrdenamiento[]
        {
            Id, NombreCompleto, Adeudo,
            UltimoMesPagado, Seccion, Calle,
            Numero, UltimoPago, FechaRegistro
        };
    }
}
