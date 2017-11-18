using AguaSB.Nucleo;
using AguaSB.Utilerias;
using System.Collections.Generic;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Columna : Notificante
    {
        private bool activa = true;

        public bool Activo
        {
            get { return activa; }
            set { N.Set(ref activa, value); }
        }

        public string Nombre { get; set; }
    }

    public class Columnas : Notificante
    {
        public Columna Contratos { get; } = new Columna { Nombre = nameof(Contratos) };

        public Columna FechaRegistro { get; } = new Columna { Nombre = nameof(FechaRegistro) };

        public Columna UltimoPago { get; } = new Columna { Nombre = nameof(UltimoPago) };

        public Columna UltimoMesPagado { get; } = new Columna { Nombre = nameof(UltimoMesPagado) };

        public Columna Seccion { get; } = new Columna { Nombre = nameof(Seccion) };

        public Columna Calle { get; } = new Columna { Nombre = nameof(Calle) };

        public Columna Numero { get; } = new Columna { Nombre = nameof(Domicilio.Numero) };

        public IEnumerable<Columna> Todas => new[] { Contratos, FechaRegistro, UltimoPago, UltimoMesPagado, Seccion, Calle, Numero };
    }
}
